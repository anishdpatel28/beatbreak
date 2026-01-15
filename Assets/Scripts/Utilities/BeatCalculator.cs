using UnityEngine;

namespace Beatbreak.Utilities
{
    /// <summary>
    /// Static utility class for beat and timing calculations
    /// </summary>
    public static class BeatCalculator
    {
        /// <summary>
        /// Converts beats to seconds based on BPM
        /// </summary>
        public static float BeatsToSeconds(float beats, float bpm)
        {
            return beats * (60f / bpm);
        }
        
        /// <summary>
        /// Converts seconds to beats based on BPM
        /// </summary>
        public static float SecondsToBeats(float seconds, float bpm)
        {
            return seconds / (60f / bpm);
        }
        
        /// <summary>
        /// Gets the duration of one beat in seconds
        /// </summary>
        public static float GetBeatDuration(float bpm)
        {
            return 60f / bpm;
        }
        
        /// <summary>
        /// Snaps a time value to the nearest beat
        /// </summary>
        public static float SnapToNearestBeat(float time, float bpm)
        {
            float beat = SecondsToBeats(time, bpm);
            float snappedBeat = Mathf.Round(beat);
            return BeatsToSeconds(snappedBeat, bpm);
        }
        
        /// <summary>
        /// Snaps a time value to a subdivision of beats (e.g., 0.5 for eighth notes)
        /// </summary>
        public static float SnapToBeatSubdivision(float time, float bpm, float subdivision)
        {
            float beat = SecondsToBeats(time, bpm);
            float snappedBeat = Mathf.Round(beat / subdivision) * subdivision;
            return BeatsToSeconds(snappedBeat, bpm);
        }
        
        /// <summary>
        /// Calculates the approach angle from a cardinal direction
        /// </summary>
        public static float GetCardinalAngle(CardinalDirection direction)
        {
            switch (direction)
            {
                case CardinalDirection.Right: return 0f;
                case CardinalDirection.Up: return 90f;
                case CardinalDirection.Left: return 180f;
                case CardinalDirection.Down: return 270f;
                case CardinalDirection.UpRight: return 45f;
                case CardinalDirection.UpLeft: return 135f;
                case CardinalDirection.DownLeft: return 225f;
                case CardinalDirection.DownRight: return 315f;
                default: return 0f;
            }
        }
    }
    
    /// <summary>
    /// Cardinal directions for note spawning
    /// </summary>
    public enum CardinalDirection
    {
        Right,
        Up,
        Left,
        Down,
        UpRight,
        UpLeft,
        DownLeft,
        DownRight
    }
}
