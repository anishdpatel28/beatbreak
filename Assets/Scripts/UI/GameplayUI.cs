using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Beatbreak.Core;

namespace Beatbreak.UI
{
    /// <summary>
    /// Main UI controller for displaying gameplay information
    /// </summary>
    public class GameplayUI : MonoBehaviour
    {
        [Header("Score Display")]
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI comboText;
        [SerializeField] private TextMeshProUGUI accuracyText;
        
        [Header("Hit Result Display")]
        [SerializeField] private TextMeshProUGUI hitResultText;
        [SerializeField] private float hitResultDisplayTime = 0.5f;
        
        [Header("Progress Bar")]
        [SerializeField] private ProgressBarUI progressBar;
        
        [Header("Practice Mode Display")]
        [SerializeField] private TextMeshProUGUI practiceModeText;
        [SerializeField] private GameObject practiceControlsPanel;
        
        [Header("Pause Menu")]
        [SerializeField] private GameObject pauseMenuPanel;
        
        private float hitResultTimer;
        private bool isPaused;
        
        private void Update()
        {
            // Update hit result display timer
            if (hitResultTimer > 0)
            {
                hitResultTimer -= Time.deltaTime;
                if (hitResultTimer <= 0 && hitResultText != null)
                {
                    hitResultText.gameObject.SetActive(false);
                }
            }
        }
        
        /// <summary>
        /// Updates the score display
        /// </summary>
        public void UpdateScore(int score)
        {
            if (scoreText != null)
            {
                scoreText.text = $"Score: {score:N0}";
            }
        }
        
        /// <summary>
        /// Updates the combo display
        /// </summary>
        public void UpdateCombo(int combo)
        {
            if (comboText != null)
            {
                if (combo > 0)
                {
                    comboText.gameObject.SetActive(true);
                    comboText.text = $"Combo: {combo}x";
                }
                else
                {
                    comboText.gameObject.SetActive(false);
                }
            }
        }
        
        /// <summary>
        /// Updates the accuracy display
        /// </summary>
        public void UpdateAccuracy(float accuracy)
        {
            if (accuracyText != null)
            {
                accuracyText.text = $"Accuracy: {accuracy:F1}%";
            }
        }
        
        /// <summary>
        /// Displays a hit result (Perfect, Good, Miss)
        /// </summary>
        public void ShowHitResult(HitResult result)
        {
            if (hitResultText == null) return;
            
            hitResultText.gameObject.SetActive(true);
            
            switch (result)
            {
                case HitResult.Perfect:
                    hitResultText.text = "PERFECT!";
                    hitResultText.color = Color.yellow;
                    break;
                case HitResult.Good:
                    hitResultText.text = "GOOD";
                    hitResultText.color = Color.green;
                    break;
                case HitResult.Miss:
                    hitResultText.text = "MISS";
                    hitResultText.color = Color.red;
                    break;
            }
            
            hitResultTimer = hitResultDisplayTime;
        }
        
        /// <summary>
        /// Updates the progress bar
        /// </summary>
        public void UpdateProgress(float currentTime, float totalTime)
        {
            if (progressBar != null)
            {
                progressBar.UpdateProgress(currentTime, totalTime);
            }
        }
        
        /// <summary>
        /// Sets practice mode UI visibility
        /// </summary>
        public void SetPracticeModeUI(bool isEnabled)
        {
            if (practiceModeText != null)
            {
                practiceModeText.gameObject.SetActive(isEnabled);
                practiceModeText.text = "PRACTICE MODE";
            }
            
            if (practiceControlsPanel != null)
            {
                practiceControlsPanel.SetActive(isEnabled);
            }
        }
        
        /// <summary>
        /// Shows or hides the pause menu
        /// </summary>
        public void SetPauseMenu(bool show)
        {
            isPaused = show;
            if (pauseMenuPanel != null)
            {
                pauseMenuPanel.SetActive(show);
            }
        }
        
        /// <summary>
        /// Gets the progress bar for checkpoint visualization
        /// </summary>
        public ProgressBarUI GetProgressBar()
        {
            return progressBar;
        }
        
        /// <summary>
        /// Initializes the UI
        /// </summary>
        public void Initialize()
        {
            UpdateScore(0);
            UpdateCombo(0);
            UpdateAccuracy(100f);
            SetPauseMenu(false);
            
            if (hitResultText != null)
            {
                hitResultText.gameObject.SetActive(false);
            }
        }
    }
}
