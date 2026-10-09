using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using GemBlast.Core;
using GemBlast.View;

namespace GemBlast.Controllers
{
    public class GameManager : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private GameConfig gameConfig;
        
        [Header("References")]
        [SerializeField] private BoardView boardView;
        [SerializeField] private Camera mainCamera;
        
        [Header("Input")]
        [SerializeField] private InputActionAsset inputActions;
        
        private InputAction _clickAction;
        private InputAction _pointAction;
        
        private Board _board;
        private MatchFinder _matchFinder;
        private BoardMechanics _mechanics;
        private RuleEngine _ruleEngine;
        
        private int _score;
        private int _movesLeft;
        
        [Header("Events")]
        [SerializeField] private GemBlast.Architecture.IntGameEvent scoreChangedEvent;
        [SerializeField] private GemBlast.Architecture.IntGameEvent movesChangedEvent;
        [SerializeField] private GemBlast.Architecture.GameEvent gameOverEvent;

        private bool _isProcessing;
        
        private List<BlockData> _matchBuffer = new List<BlockData>(64);

        private void Awake()
        {
            SetupInput();
        }

        private void OnEnable()
        {
            _clickAction?.Enable();
            _pointAction?.Enable();
        }

        private void OnDisable()
        {
            _clickAction?.Disable();
            _pointAction?.Disable();
        }

        private void SetupInput()
        {
            if (inputActions == null) return;
            
            var gameplayMap = inputActions.FindActionMap("Gameplay");
            if (gameplayMap != null)
            {
                _clickAction = gameplayMap.FindAction("Click");
                _pointAction = gameplayMap.FindAction("Point");
                
                if (_clickAction != null)
                {
                    _clickAction.performed += OnClickPerformed;
                }
            }
        }

        private void OnDestroy()
        {
            if (_clickAction != null)
            {
                _clickAction.performed -= OnClickPerformed;
            }
        }

        private WaitForSeconds _blastWait;
        private WaitForSeconds _settleWait;

        private void OnClickPerformed(InputAction.CallbackContext context)
        {
            if (_isProcessing) return;
            if (_movesLeft <= 0) return;
            
            Vector2 screenPos = _pointAction?.ReadValue<Vector2>() ?? Vector2.zero;
            HandleInput(screenPos);
        }

        private void Start()
        {
            InitializeGame();
        }

        private void InitializeGame()
        {
            if (mainCamera == null) mainCamera = Camera.main;

            _blastWait = new WaitForSeconds(gameConfig.blastDelay);
            _settleWait = new WaitForSeconds(gameConfig.settleDelay);

            _score = 0;
            _movesLeft = gameConfig.maxMoves;
            
            scoreChangedEvent?.Raise(_score);
            movesChangedEvent?.Raise(_movesLeft);

            _board = new Board(gameConfig.columns, gameConfig.rows);
            _matchFinder = new MatchFinder(_board);
            _mechanics = new BoardMechanics(_board);
            _ruleEngine = new RuleEngine(gameConfig.thresholdA, gameConfig.thresholdB, gameConfig.thresholdC);

            _board.FillRandom(gameConfig.colorCount, System.Environment.TickCount);
            
            _mechanics.SetNextBlockId(_board.Width * _board.Height + 1);
            
            if (_mechanics.IsDeadlocked(gameConfig.minMatchSize))
            {
                _mechanics.ShuffleBoard(gameConfig.minMatchSize);
            }
            
            UpdateIcons();

            boardView.Initialize(_board);
            boardView.RefreshAll();
        }
        
        public Board Board => _board;
        public bool IsProcessing => _isProcessing;
        public int Score => _score;
        public int MovesLeft => _movesLeft;

        public void HandleInput(Vector3 screenPos)
        {
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(screenPos);
            Vector2Int gridPos = boardView.GetGridPosition(worldPos);

            if (_board.IsValidCoordinate(gridPos.x, gridPos.y))
            {
                BlockData clickedBlock = _board.GetBlock(gridPos.x, gridPos.y);
                if (clickedBlock.IsValid)
                {
                    TryBlast(clickedBlock);
                }
            }
        }

        public void TryBlast(BlockData block)
        {
            if (_movesLeft <= 0) return;

            _matchBuffer.Clear();
            _matchFinder.FindMatchesNonAlloc(block.X, block.Y, _matchBuffer);
            
            if (_matchBuffer.Count >= gameConfig.minMatchSize)
            {
                _movesLeft--;
                movesChangedEvent?.Raise(_movesLeft);
                
                int points = _matchBuffer.Count * _matchBuffer.Count * gameConfig.pointsPerBlockSquared;
                _score += points;
                scoreChangedEvent?.Raise(_score);
                
                StartCoroutine(ProcessTurn(_matchBuffer, points));
            }
            else
            {
                boardView.AnimateInvalidInput(block);
                ShakeCamera(0.05f, 0.15f);
            }
        }

        private IEnumerator ProcessTurn(List<BlockData> matches, int points)
        {
            _isProcessing = true;
            try
            {
                bool isBigBlast = matches.Count >= gameConfig.bigBlastSize;
                boardView.AnimateBlast(matches, points, isBigBlast);
                
                if (GemBlast.Audio.SoundManager.Instance != null)
                    GemBlast.Audio.SoundManager.Instance.PlayBlast(isBigBlast);
            
                float shakeStr = isBigBlast ? 0.3f : 0.1f;
                ShakeCamera(shakeStr, 0.2f);
            
                yield return _blastWait;

                _mechanics.Blast(matches);
            
                _mechanics.ApplyGravity();
                _mechanics.RefillBoard(gameConfig.colorCount, System.Environment.TickCount);
            
                UpdateIcons();

                boardView.OnBoardUpdated();
                
                if (GemBlast.Audio.SoundManager.Instance != null)
                    GemBlast.Audio.SoundManager.Instance.PlayDrop();

                yield return _settleWait;

                if (_mechanics.IsDeadlocked(gameConfig.minMatchSize))
                {
                    yield return _settleWait;
                    
                    _mechanics.ShuffleBoard(gameConfig.minMatchSize);
                    UpdateIcons();
                    boardView.OnBoardUpdated();
                    
                    yield return _settleWait;
                }
            
                if (_movesLeft <= 0)
                {
                    boardView.HideAllBlocks();
                    gameOverEvent?.Raise();
                }
            }
            finally
            {
                _isProcessing = false;
            }
        }

        private void UpdateIcons()
        {
            _matchFinder.ComputeAllGroups();
            
            for (int y = 0; y < _board.Height; y++)
            {
                for (int x = 0; x < _board.Width; x++)
                {
                    BlockData b = _board.GetBlock(x, y);
                    if (b.IsValid)
                    {
                        int groupSize = _matchFinder.GetGroupSize(x, y);
                        IconType icon = _ruleEngine.GetIconTypeForGroupSize(groupSize);
                        
                        if (b.Icon != icon)
                        {
                            BlockData updated = b.WithIcon(icon);
                            _board.SetBlock(x, y, updated);
                        }
                    }
                }
            }
        }

        private void ShakeCamera(float intensity, float duration)
        {
            if (mainCamera != null)
            {
                PrimeTween.Tween.ShakeLocalPosition(mainCamera.transform, new Vector3(intensity, intensity, 0), duration, 10);
            }
        }
    }
}
