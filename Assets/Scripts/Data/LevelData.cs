using UnityEngine;
using System.Collections.Generic;

namespace Beatbreak.Data
{
    /// <summary>
    /// ScriptableObject that defines a complete level with all its hit objects
    /// </summary>
    [CreateAssetMenu(fileName = "NewLevel", menuName = "Beatbreak/Level Data")]
    public class LevelData : ScriptableObject
    {
        [Header("Song Settings")]
        [Tooltip("The audio clip for this level")]
        public AudioClip song;
        
        [Tooltip("Beats per minute of the song")]
        public float bpm = 120f;
        
        [Tooltip("Offset in seconds before the song starts (for sync adjustment)")]
        public float songOffset = 0f;
        
        [Header("Level Info")]
        [Tooltip("Display name of the level")]
        public string levelName = "Unnamed Level";
        
        [Tooltip("Level description or artist info")]
        [TextArea(2, 4)]
        public string description;
        
        [Header("Gameplay Settings")]
        [Tooltip("Default approach time in seconds (time from spawn to hit)")]
        public float defaultApproachTime = 1f;
        
        [Tooltip("All hit objects in this level")]
        public List<HitObjectData> hitObjects = new List<HitObjectData>();
        
        /// <summary>
        /// Gets the duration of one beat in seconds
        /// </summary>
        public float GetBeatDuration()
        {
            return 60f / bpm;
        }
        
        /// <summary>
        /// Gets the scaled BPM based on difficulty speed multiplier
        /// </summary>
        public float GetScaledBPM(float speedMultiplier)
        {
            return bpm * speedMultiplier;
        }
        
        /// <summary>
        /// Gets the total song duration
        /// </summary>
        public float GetSongDuration()
        {
            return song != null ? song.length : 0f;
        }
    }
}
