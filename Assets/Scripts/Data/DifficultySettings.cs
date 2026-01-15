using UnityEngine;

namespace Beatbreak.Data
{
    /// <summary>
    /// ScriptableObject that defines difficulty modifiers
    /// </summary>
    [CreateAssetMenu(fileName = "NewDifficulty", menuName = "Beatbreak/Difficulty Settings")]
    public class DifficultySettings : ScriptableObject
    {
        [Header("Difficulty Info")]
        [Tooltip("Name of this difficulty (e.g., Easy, Normal, Hard)")]
        public string difficultyName = "Normal";
        
        [Header("Speed Settings")]
        [Tooltip("Speed multiplier for the song and notes")]
        [Range(0.25f, 3f)]
        public float speedMultiplier = 1f;
        
        [Header("Hit Window Settings (in seconds)")]
        [Tooltip("Time window for a perfect hit")]
        public float perfectWindow = 0.05f;
        
        [Tooltip("Time window for a good hit")]
        public float goodWindow = 0.1f;
        
        [Tooltip("Apply speed scaling to hit windows")]
        public bool scaleHitWindowsWithSpeed = true;
        
        /// <summary>
        /// Gets the scaled perfect window based on speed multiplier
        /// </summary>
        public float GetScaledPerfectWindow()
        {
            return scaleHitWindowsWithSpeed ? perfectWindow / speedMultiplier : perfectWindow;
        }
        
        /// <summary>
        /// Gets the scaled good window based on speed multiplier
        /// </summary>
        public float GetScaledGoodWindow()
        {
            return scaleHitWindowsWithSpeed ? goodWindow / speedMultiplier : goodWindow;
        }
    }
}
