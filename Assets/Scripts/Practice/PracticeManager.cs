using UnityEngine;
using System.Collections.Generic;
using Beatbreak.Data;
using Beatbreak.Core;
using Beatbreak.Gameplay;

namespace Beatbreak.Practice
{
    /// <summary>
    /// Manages practice mode checkpoints and restarting from saved positions
    /// </summary>
    public class PracticeManager : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private bool isPracticeMode = true;
        [SerializeField] private KeyCode createCheckpointKey = KeyCode.Z;
        [SerializeField] private KeyCode removeCheckpointKey = KeyCode.X;
        [SerializeField] private KeyCode previousCheckpointKey = KeyCode.LeftBracket;
        [SerializeField] private KeyCode nextCheckpointKey = KeyCode.RightBracket;
        
        private List<CheckpointData> checkpoints = new List<CheckpointData>();
        private int currentCheckpointIndex = -1;
        
        // References (set by GameplayManager)
        private TimingSystem timingSystem;
        private AudioSource audioSource;
        private NoteSpawner noteSpawner;
        
        // Events
        public event System.Action<CheckpointData> OnCheckpointCreated;
        public event System.Action<int> OnCheckpointRemoved;
        public event System.Action<CheckpointData> OnCheckpointActivated;
        
        public bool IsPracticeMode => isPracticeMode;
        public List<CheckpointData> Checkpoints => checkpoints;
        public int CurrentCheckpointIndex => currentCheckpointIndex;
        
        private void Update()
        {
            if (!isPracticeMode) return;
            
            HandleInput();
        }
        
        /// <summary>
        /// Initializes the practice manager with required references
        /// </summary>
        public void Initialize(TimingSystem timing, AudioSource audio, NoteSpawner spawner)
        {
            timingSystem = timing;
            audioSource = audio;
            noteSpawner = spawner;
            checkpoints.Clear();
            currentCheckpointIndex = -1;
        }
        
        /// <summary>
        /// Handles keyboard input for checkpoint management
        /// </summary>
        private void HandleInput()
        {
            // Create checkpoint
            if (Input.GetKeyDown(createCheckpointKey))
            {
                CreateCheckpoint();
            }
            
            // Remove latest checkpoint
            if (Input.GetKeyDown(removeCheckpointKey))
            {
                RemoveLatestCheckpoint();
            }
            
            // Go to previous checkpoint
            if (Input.GetKeyDown(previousCheckpointKey))
            {
                GoToPreviousCheckpoint();
            }
            
            // Go to next checkpoint
            if (Input.GetKeyDown(nextCheckpointKey))
            {
                GoToNextCheckpoint();
            }
        }
        
        /// <summary>
        /// Creates a checkpoint at the current time
        /// </summary>
        public void CreateCheckpoint()
        {
            if (timingSystem == null) return;
            
            float currentTime = timingSystem.CurrentSongTime;
            int nextHitIndex = noteSpawner.GetNextSpawnIndex();
            
            CheckpointData newCheckpoint = new CheckpointData(currentTime, nextHitIndex);
            
            // Insert checkpoint in chronological order
            int insertIndex = checkpoints.Count;
            for (int i = 0; i < checkpoints.Count; i++)
            {
                if (checkpoints[i].songTime > currentTime)
                {
                    insertIndex = i;
                    break;
                }
            }
            
            checkpoints.Insert(insertIndex, newCheckpoint);
            currentCheckpointIndex = insertIndex;
            
            OnCheckpointCreated?.Invoke(newCheckpoint);
            
            Debug.Log($"Checkpoint created at {currentTime:F2}s (Index: {nextHitIndex})");
        }
        
        /// <summary>
        /// Removes the most recently created checkpoint
        /// </summary>
        public void RemoveLatestCheckpoint()
        {
            if (checkpoints.Count == 0) return;
            
            int lastIndex = checkpoints.Count - 1;
            checkpoints.RemoveAt(lastIndex);
            
            if (currentCheckpointIndex >= checkpoints.Count)
            {
                currentCheckpointIndex = checkpoints.Count - 1;
            }
            
            OnCheckpointRemoved?.Invoke(lastIndex);
            
            Debug.Log($"Checkpoint removed. Remaining: {checkpoints.Count}");
        }
        
        /// <summary>
        /// Goes to the previous checkpoint
        /// </summary>
        public void GoToPreviousCheckpoint()
        {
            if (checkpoints.Count == 0) return;
            
            currentCheckpointIndex--;
            if (currentCheckpointIndex < 0)
            {
                currentCheckpointIndex = 0;
                RestartFromBeginning();
            }
            else
            {
                RestartFromCheckpoint(checkpoints[currentCheckpointIndex]);
            }
        }
        
        /// <summary>
        /// Goes to the next checkpoint
        /// </summary>
        public void GoToNextCheckpoint()
        {
            if (checkpoints.Count == 0) return;
            
            currentCheckpointIndex++;
            if (currentCheckpointIndex >= checkpoints.Count)
            {
                currentCheckpointIndex = checkpoints.Count - 1;
                return; // Already at the last checkpoint
            }
            
            RestartFromCheckpoint(checkpoints[currentCheckpointIndex]);
        }
        
        /// <summary>
        /// Restarts from a specific checkpoint
        /// </summary>
        public void RestartFromCheckpoint(CheckpointData checkpoint)
        {
            if (audioSource == null || noteSpawner == null || timingSystem == null)
                return;
            
            // Stop the song
            audioSource.Stop();
            
            // Clear all active hit objects
            noteSpawner.ClearAllHitObjects();
            
            // Reset to checkpoint's hit object index
            noteSpawner.ResetToIndex(checkpoint.hitObjectIndex);
            
            // Restart timing from checkpoint
            timingSystem.StartTimingFromOffset(checkpoint.songTime);
            
            // Restart audio from checkpoint
            audioSource.time = checkpoint.songTime;
            audioSource.Play();
            
            OnCheckpointActivated?.Invoke(checkpoint);
            
            Debug.Log($"Restarted from checkpoint at {checkpoint.songTime:F2}s");
        }
        
        /// <summary>
        /// Restarts from the beginning
        /// </summary>
        private void RestartFromBeginning()
        {
            if (audioSource == null || noteSpawner == null || timingSystem == null)
                return;
            
            // Stop the song
            audioSource.Stop();
            
            // Clear all active hit objects
            noteSpawner.ClearAllHitObjects();
            
            // Reset to beginning
            noteSpawner.ResetToIndex(0);
            timingSystem.StartTiming();
            
            // Restart audio
            audioSource.time = 0f;
            audioSource.Play();
            
            Debug.Log("Restarted from beginning");
        }
        
        /// <summary>
        /// Enables or disables practice mode
        /// </summary>
        public void SetPracticeMode(bool enabled)
        {
            isPracticeMode = enabled;
            if (!enabled)
            {
                checkpoints.Clear();
                currentCheckpointIndex = -1;
            }
        }
        
        /// <summary>
        /// Gets the checkpoint closest to a given time
        /// </summary>
        public CheckpointData GetCheckpointAtTime(float time)
        {
            CheckpointData closest = null;
            float closestDistance = float.MaxValue;
            
            foreach (var checkpoint in checkpoints)
            {
                float distance = Mathf.Abs(checkpoint.songTime - time);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closest = checkpoint;
                }
            }
            
            return closest;
        }
    }
}
