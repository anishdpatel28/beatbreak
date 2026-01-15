using UnityEngine;
using Beatbreak.Data;

namespace Beatbreak.Core
{
    /// <summary>
    /// Manages scoring and hit detection timing windows
    /// </summary>
    public class ScoringSystem
    {
        private DifficultySettings currentDifficulty;
        private int perfectHits;
        private int goodHits;
        private int missedHits;
        private int combo;
        private int maxCombo;
        private int totalScore;
        
        // Score values
        private const int PERFECT_SCORE = 300;
        private const int GOOD_SCORE = 100;
        private const int COMBO_MULTIPLIER = 10;
        
        // Events for score updates
        public event System.Action<HitResult, int> OnHit;
        public event System.Action<int> OnComboChanged;
        public event System.Action<int> OnScoreChanged;
        
        // Properties
        public int PerfectHits => perfectHits;
        public int GoodHits => goodHits;
        public int MissedHits => missedHits;
        public int Combo => combo;
        public int MaxCombo => maxCombo;
        public int TotalScore => totalScore;
        public float Accuracy => GetAccuracy();
        
        /// <summary>
        /// Initializes the scoring system with difficulty settings
        /// </summary>
        public void Initialize(DifficultySettings difficulty)
        {
            currentDifficulty = difficulty;
            ResetScore();
        }
        
        /// <summary>
        /// Resets all score values
        /// </summary>
        public void ResetScore()
        {
            perfectHits = 0;
            goodHits = 0;
            missedHits = 0;
            combo = 0;
            maxCombo = 0;
            totalScore = 0;
        }
        
        /// <summary>
        /// Evaluates the timing of a hit and returns the result
        /// </summary>
        public HitResult EvaluateHit(float timingError)
        {
            float absError = Mathf.Abs(timingError);
            
            if (absError <= currentDifficulty.GetScaledPerfectWindow())
            {
                return HitResult.Perfect;
            }
            else if (absError <= currentDifficulty.GetScaledGoodWindow())
            {
                return HitResult.Good;
            }
            
            return HitResult.Miss;
        }
        
        /// <summary>
        /// Registers a hit result and updates score
        /// </summary>
        public void RegisterHit(HitResult result)
        {
            int scoreGained = 0;
            
            switch (result)
            {
                case HitResult.Perfect:
                    perfectHits++;
                    combo++;
                    scoreGained = PERFECT_SCORE + (combo * COMBO_MULTIPLIER);
                    break;
                    
                case HitResult.Good:
                    goodHits++;
                    combo++;
                    scoreGained = GOOD_SCORE + (combo * COMBO_MULTIPLIER);
                    break;
                    
                case HitResult.Miss:
                    missedHits++;
                    combo = 0;
                    scoreGained = 0;
                    break;
            }
            
            if (combo > maxCombo)
            {
                maxCombo = combo;
            }
            
            totalScore += scoreGained;
            
            // Trigger events
            OnHit?.Invoke(result, scoreGained);
            OnComboChanged?.Invoke(combo);
            OnScoreChanged?.Invoke(totalScore);
        }
        
        /// <summary>
        /// Calculates the current accuracy percentage
        /// </summary>
        private float GetAccuracy()
        {
            int totalHits = perfectHits + goodHits + missedHits;
            if (totalHits == 0) return 100f;
            
            float weightedScore = (perfectHits * 1f) + (goodHits * 0.5f);
            return (weightedScore / totalHits) * 100f;
        }
        
        /// <summary>
        /// Gets a score summary for display
        /// </summary>
        public ScoreData GetScoreData()
        {
            return new ScoreData
            {
                totalScore = totalScore,
                perfectHits = perfectHits,
                goodHits = goodHits,
                missedHits = missedHits,
                maxCombo = maxCombo,
                accuracy = GetAccuracy()
            };
        }
    }
    
    /// <summary>
    /// Data structure for final score display
    /// </summary>
    [System.Serializable]
    public struct ScoreData
    {
        public int totalScore;
        public int perfectHits;
        public int goodHits;
        public int missedHits;
        public int maxCombo;
        public float accuracy;
    }
}
