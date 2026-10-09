using UnityEngine;
using System.Collections.Generic;
using GemBlast.Core;
using PrimeTween;

namespace GemBlast.View
{
    public class BoardView : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private BlockView blockPrefab;
        [SerializeField] private Transform boardContainer;

        [System.Serializable]
        public struct ColorAsset
        {
            public Sprite iconDefault;
            public Sprite iconA;
            public Sprite iconB;
            public Sprite iconC;
        }

        [Header("Assets")]
        [SerializeField] private ColorAsset[] colorAssets;

        [Header("Layout")]
        [SerializeField] private float cellSize = 1.0f;
        
        private Dictionary<int, BlockView> _activeBlocks = new Dictionary<int, BlockView>();
        private Queue<BlockView> _pool = new Queue<BlockView>();
        private HashSet<int> _liveIds = new HashSet<int>(); 
        private List<int> _deadIds = new List<int>();

        private Board _board;
        private BlastEffects _effects;
        private float _halfWidth, _halfHeight, _baseOrthoSize;
        private int _lastScreenW, _lastScreenH;

        public void Initialize(Board board)
        {
            Clear();
            
            _board = board;
            _effects ??= new BlastEffects(boardContainer != null ? boardContainer : transform);
            float totalWidth = board.Width * cellSize;
            float totalHeight = board.Height * cellSize;
            
            float offsetX = -totalWidth / 2f + cellSize / 2f;
            float offsetY = -totalHeight / 2f + cellSize / 2f;
            
            transform.position = new Vector3(offsetX, offsetY, -1f);
            
            _halfWidth = totalWidth / 2f + 0.5f;
            _halfHeight = totalHeight / 2f + 1f;

            Camera cam = Camera.main;
            if (cam != null)
            {
                 var pos = cam.transform.position;
                 cam.transform.position = new Vector3(pos.x, pos.y, -10f);
                 _baseOrthoSize = cam.orthographicSize;
                 FitCamera();
            }
        }

        // Keep the whole board visible at any resolution / aspect ratio.
        private void Update()
        {
            if (_board == null || (Screen.width == _lastScreenW && Screen.height == _lastScreenH)) return;
            FitCamera();
        }

        private void FitCamera()
        {
            Camera cam = Camera.main;
            if (cam == null) return;

            _lastScreenW = Screen.width;
            _lastScreenH = Screen.height;
            cam.orthographicSize = Mathf.Max(_baseOrthoSize, _halfHeight, _halfWidth / cam.aspect);
        }

        public void Clear()
        {
            foreach (var view in _activeBlocks.Values)
            {
                view.gameObject.SetActive(false);
                _pool.Enqueue(view);
            }
            _activeBlocks.Clear();
            _liveIds.Clear();
            _deadIds.Clear();
        }

        public void RefreshAll()
        {
            for (int y = 0; y < _board.Height; y++)
            {
                for (int x = 0; x < _board.Width; x++)
                {
                    BlockData data = _board.GetBlock(x, y);
                    if (data.IsValid)
                    {
                        CreateOrUpdateBlock(data, x, y);
                    }
                }
            }
        }
        
        public void OnBoardUpdated()
        {
            _liveIds.Clear();
            _deadIds.Clear();

            for (int y = 0; y < _board.Height; y++)
            {
                for (int x = 0; x < _board.Width; x++)
                {
                    var b = _board.GetBlock(x, y);
                    if (b.IsValid) 
                    {
                        _liveIds.Add(b.Id);
                        CreateOrUpdateBlock(b, x, y);
                    }
                }
            }
            
            foreach (var key in _activeBlocks.Keys)
            {
                if (!_liveIds.Contains(key)) _deadIds.Add(key);
            }
            
            foreach (var id in _deadIds)
            {
                Recycle(id);
            }
        }

        private void CreateOrUpdateBlock(BlockData data, int x, int y)
        {
            Vector3 localPos = GetLocalPosition(x, y);
            
            if (_activeBlocks.TryGetValue(data.Id, out BlockView view))
            {
                if (Vector3.SqrMagnitude(view.transform.localPosition - localPos) > 0.0001f)
                {
                    view.MoveToProperties(x, y, localPos);
                }
                
                if (view.Data.ColorIndex != data.ColorIndex || view.Data.Icon != data.Icon)
                {
                    Sprite s = GetSprite(data.ColorIndex, data.Icon);
                    view.UpdateVisuals(s);
                }
                
                view.UpdateData(data);
            }
            else
            {
                BlockView newView = GetFromPool();
                Sprite s = GetSprite(data.ColorIndex, data.Icon);
                
                float spawnY = (_board.Height * cellSize); 
                Vector3 spawnLocalPos = new Vector3(localPos.x, localPos.y + (_board.Height * cellSize), localPos.z);
                
                newView.transform.localPosition = spawnLocalPos;
                newView.Initialize(data, s, y + 10);
                _activeBlocks.Add(data.Id, newView);
                
                newView.MoveToProperties(x, y, localPos);
            }
        }

        private void Recycle(int id)
        {
            if (_activeBlocks.TryGetValue(id, out BlockView view))
            {
                view.gameObject.SetActive(false);
                _pool.Enqueue(view);
                _activeBlocks.Remove(id);
            }
        }

        private BlockView GetFromPool()
        {
            BlockView view;
            if (_pool.Count > 0)
            {
                view = _pool.Dequeue();
            }
            else
            {
                view = Instantiate(blockPrefab, boardContainer);
            }
            
            view.transform.localScale = Vector3.one;
            view.transform.rotation = Quaternion.identity;
            Tween.StopAll(view.transform);
            view.gameObject.SetActive(true);
            return view;
        }

        public Vector3 GetLocalPosition(int x, int y)
        {
            return new Vector3(x * cellSize, y * cellSize, 0);
        }
        
        public Vector2Int GetGridPosition(Vector3 worldPos)
        {
            Vector3 localPos = transform.InverseTransformPoint(worldPos);
            
            int x = Mathf.RoundToInt(localPos.x / cellSize);
            int y = Mathf.RoundToInt(localPos.y / cellSize);
            return new Vector2Int(x, y);
        }

        private Sprite GetSprite(int colorIndex, IconType icon)
        {
            int assetIndex = colorIndex - 1;
            if (assetIndex < 0 || assetIndex >= colorAssets.Length) return null;
            
            ColorAsset asset = colorAssets[assetIndex];
            switch (icon)
            {
                case IconType.IconA: return asset.iconA;
                case IconType.IconB: return asset.iconB;
                case IconType.IconC: return asset.iconC;
                default: return asset.iconDefault;
            }
        }

        public void AnimateInvalidInput(BlockData data)
        {
            if (_activeBlocks.TryGetValue(data.Id, out BlockView view))
            {
                view.AnimatePunch();
            }
        }

        public void AnimateBlast(List<BlockData> blocks, int points, bool isBig = false)
        {
            Vector3 center = Vector3.zero;
            int found = 0;

            foreach (var block in blocks)
            {
                if (_activeBlocks.TryGetValue(block.Id, out BlockView view))
                {
                    view.AnimateBlast(isBig);
                    _effects.SpawnShards(view.transform.position, GetSprite(block.ColorIndex, block.Icon), isBig ? 1.6f : 1f);
                    center += view.transform.position;
                    found++;
                }
            }

            if (found > 0)
                _effects.ShowScorePopup(center / found, points, isBig);
        }

        public void HideAllBlocks()
        {
            foreach (var view in _activeBlocks.Values)
            {
                view.gameObject.SetActive(false);
            }
        }
    }
}
