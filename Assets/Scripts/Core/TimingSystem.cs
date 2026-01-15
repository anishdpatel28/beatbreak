using UnityEngine;
using Beatbreak.Data;

namespace Beatbreak.Core
{
    /// <summary>
    /// Manages precise timing for the rhythm game using AudioSettings.dspTime
    /// </summary>
    public class TimingSystem
    {
        private double songStartDspTime;
        private float currentSpeedMultiplier = 1f;
        private float currentBPM;
        private float songOffset;
        private bool isPlaying;
        
        /// <summary>
        /// Gets the current song time in seconds
        /// </summary>
        public float CurrentSongTime
        {
            get
            {
                if (!isPlaying) return 0f;
                return (float)(AudioSettings.dspTime - songStartDspTime);
            }
        }
        
        /// <summary>
        /// Gets the current beat based on song time
        /// </summary>
        public float CurrentBeat
        {
            get
            {
                if (currentBPM <= 0) return 0f;
                return (CurrentSongTime - songOffset) / (60f / currentBPM);
            }
        }
        
        /// <summary>
        /// Gets the duration of one beat in seconds
        /// </summary>
        public float BeatDuration => 60f / currentBPM;
        
        /// <summary>
        /// Gets the current BPM
        /// </summary>
        public float CurrentBPM => currentBPM;
        
        /// <summary>
        /// Gets the current speed multiplier
        /// </summary>
        public float SpeedMultiplier => currentSpeedMultiplier;
        
        /// <summary>
        /// Initializes the timing system with level and difficulty settings
        /// </summary>
        public void Initialize(LevelData levelData, DifficultySettings difficulty)
        {
            currentBPM = levelData.GetScaledBPM(difficulty.speedMultiplier);
            currentSpeedMultiplier = difficulty.speedMultiplier;
            songOffset = levelData.songOffset;
            isPlaying = false;
        }
        
        /// <summary>
        /// Starts the timing system
        /// </summary>
        public void StartTiming()
        {
            songStartDspTime = AudioSettings.dspTime;
            isPlaying = true;
        }
        
        /// <summary>
        /// Starts timing from a specific offset (for checkpoints)
        /// </summary>
        public void StartTimingFromOffset(float timeOffset)
        {
            songStartDspTime = AudioSettings.dspTime - timeOffset;
            isPlaying = true;
        }
        
        /// <summary>
        /// Stops the timing system
        /// </summary>
        public void StopTiming()
        {
            isPlaying = false;
        }
        
        /// <summary>
        /// Converts a beat time to seconds
        /// </summary>
        public float BeatsToSeconds(float beats)
        {
            return beats * (60f / currentBPM) + songOffset;
        }
        
        /// <summary>
        /// Converts seconds to beat time
        /// </summary>
        public float SecondsToBeats(float seconds)
        {
            return (seconds - songOffset) / (60f / currentBPM);
        }
        
        /// <summary>
        /// Checks if a specific beat time has been reached
        /// </summary>
        public bool HasReachedBeat(float targetBeat)
        {
            return CurrentBeat >= targetBeat;
        }
        
        /// <summary>
        /// Gets the time until a specific beat
        /// </summary>
        public float GetTimeUntilBeat(float targetBeat)
        {
            return BeatsToSeconds(targetBeat) - CurrentSongTime;
        }
    }
}
