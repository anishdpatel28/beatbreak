using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Beatbreak.Core;

namespace Beatbreak.UI
{
    /// <summary>
    /// Displays the final results at the end of a level
    /// </summary>
    public class ResultsScreenUI : MonoBehaviour
    {
        [Header("Score Display")]
        [SerializeField] private TextMeshProUGUI totalScoreText;
        [SerializeField] private TextMeshProUGUI accuracyText;
        [SerializeField] private TextMeshProUGUI maxComboText;
        
        [Header("Hit Statistics")]
        [SerializeField] private TextMeshProUGUI perfectHitsText;
        [SerializeField] private TextMeshProUGUI goodHitsText;
        [SerializeField] private TextMeshProUGUI missedHitsText;
        
        [Header("Rank Display")]
        [SerializeField] private TextMeshProUGUI rankText;
        [SerializeField] private Image rankImage;
        
        [Header("Buttons")]
        [SerializeField] private Button retryButton;
        [SerializeField] private Button menuButton;
        
        private void Awake()
        {
            gameObject.SetActive(false);
        }
        
        /// <summary>
        /// Displays the results screen with score data
        /// </summary>
        public void ShowResults(ScoreData scoreData)
        {
            gameObject.SetActive(true);
            
            // Display scores
            if (totalScoreText != null)
                totalScoreText.text = $"Score: {scoreData.totalScore:N0}";
            
            if (accuracyText != null)
                accuracyText.text = $"Accuracy: {scoreData.accuracy:F2}%";
            
            if (maxComboText != null)
                maxComboText.text = $"Max Combo: {scoreData.maxCombo}x";
            
            // Display hit statistics
            if (perfectHitsText != null)
                perfectHitsText.text = $"Perfect: {scoreData.perfectHits}";
            
            if (goodHitsText != null)
                goodHitsText.text = $"Good: {scoreData.goodHits}";
            
            if (missedHitsText != null)
                missedHitsText.text = $"Miss: {scoreData.missedHits}";
            
            // Calculate and display rank
            string rank = CalculateRank(scoreData.accuracy);
            if (rankText != null)
            {
                rankText.text = $"Rank: {rank}";
            }
        }
        
        /// <summary>
        /// Hides the results screen
        /// </summary>
        public void HideResults()
        {
            gameObject.SetActive(false);
        }
        
        /// <summary>
        /// Calculates the rank based on accuracy
        /// </summary>
        private string CalculateRank(float accuracy)
        {
            if (accuracy >= 95f) return "S";
            if (accuracy >= 90f) return "A";
            if (accuracy >= 80f) return "B";
            if (accuracy >= 70f) return "C";
            if (accuracy >= 60f) return "D";
            return "F";
        }
        
        /// <summary>
        /// Sets up button callbacks
        /// </summary>
        public void SetupButtons(System.Action onRetry, System.Action onMenu)
        {
            if (retryButton != null)
            {
                retryButton.onClick.RemoveAllListeners();
                retryButton.onClick.AddListener(() => onRetry?.Invoke());
            }
            
            if (menuButton != null)
            {
                menuButton.onClick.RemoveAllListeners();
                menuButton.onClick.AddListener(() => onMenu?.Invoke());
            }
        }
    }
}
