using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace GemBlast.UI
{
    public class UIManager : MonoBehaviour
    {
        [Header("HUD")]
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI movesText;
        
        [Header("Game Over")]
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private TextMeshProUGUI finalScoreText;
        [SerializeField] private Button restartButton;
        
        private const string HighScoreKey = "GemBlast.HighScore";

        private int _lastScore;

        private void Start()
        {
            if (restartButton != null)
            {
                restartButton.onClick.RemoveAllListeners();
                restartButton.onClick.AddListener(OnRestartClicked);
            }
            
            if (gameOverPanel != null) gameOverPanel.SetActive(false);
        }

        public void UpdateScore(int score)
        {
            _lastScore = score;
            if (scoreText != null) scoreText.text = $"Score: {score}";
        }

        public void UpdateMoves(int moves)
        {
            if (movesText != null) movesText.text = $"Moves: {moves}";
        }

        public void ShowGameOver()
        {
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
                if (finalScoreText != null)
                {
                    int best = PlayerPrefs.GetInt(HighScoreKey, 0);
                    bool isNewBest = _lastScore > best;
                    if (isNewBest)
                    {
                        best = _lastScore;
                        PlayerPrefs.SetInt(HighScoreKey, best);
                        PlayerPrefs.Save();
                    }

                    finalScoreText.text = isNewBest
                        ? $"Final Score: {_lastScore}\nNew Best!"
                        : $"Final Score: {_lastScore}\nBest: {best}";
                }
            }
        }

        public void OnRestartClicked()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }

        public void OnExitClicked()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
    }
}
