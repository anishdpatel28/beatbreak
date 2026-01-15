using UnityEngine;

namespace Beatbreak.Data
{
    /// <summary>
    /// Defines a single hit object (note) in a level
    /// </summary>
    [System.Serializable]
    public class HitObjectData
    {
        [Tooltip("Beat time when this note should be hit")]
        public float beatTime;
        
        [Tooltip("Direction the note approaches from (0-360 degrees)")]
        [Range(0f, 360f)]
        public float approachAngle;
        
        [Tooltip("Speed multiplier for this specific note")]
        public float speedMultiplier = 1f;
        
        [Tooltip("Size multiplier for this note (1 = default)")]
        public float sizeMultiplier = 1f;
        
        [Tooltip("Additional position offset from the approach direction")]
        public Vector2 positionOffset = Vector2.zero;
        
        /// <summary>
        /// Converts beat time to seconds based on BPM
        /// </summary>
        public float GetTimeInSeconds(float bpm)
        {
            return beatTime * (60f / bpm);
        }
        
        /// <summary>
        /// Gets the spawn direction as a normalized vector
        /// </summary>
        public Vector2 GetApproachDirection()
        {
            float radians = approachAngle * Mathf.Deg2Rad;
            return new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
        }
    }
}
