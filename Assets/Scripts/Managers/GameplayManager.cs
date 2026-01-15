using UnityEngine;
using Beatbreak.Data;
using Beatbreak.Core;
using Beatbreak.Gameplay;
using Beatbreak.Practice;
using Beatbreak.UI;

namespace Beatbreak.Managers
{
    /// <summary>
    /// Main gameplay manager that orchestrates all systems
    /// </summary>
    public class GameplayManager : MonoBehaviour
    {
        [Header("Level Settings")]
        [SerializeField] private LevelData levelData;
        [SerializeField] private DifficultySettings difficultySettings;
        
        [Header("References")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private NoteSpawner noteSpawner;
        [SerializeField] private PlayerZone playerZone;
        [SerializeField] private CursorController cursorController;
        [SerializeField] private PracticeManager practiceManager;
        [SerializeField] private GameplayUI gameplayUI;
        [SerializeField] private ResultsScreenUI resultsScreen;
        
        [Header("Gameplay Settings")]
        [SerializeField] private KeyCode pauseKey = KeyCode.Escape;
        
        // Core systems
        private TimingSystem timingSystem;
        private ScoringSystem scoringSystem;
        
        // State
        private bool isPlaying;
        private bool isPaused;
        private bool levelComplete;
        
        public bool IsPlaying => isPlaying;
        public bool IsPaused => isPaused;
        public TimingSystem TimingSystem => timingSystem;
        public ScoringSystem ScoringSystem => scoringSystem;
        
        private void Awake()
        {
            // Initialize core systems
            timingSystem = new TimingSystem();
            scoringSystem = new ScoringSystem();
            
            // Validate references
            ValidateReferences();
        }
        
        private void Start()
        {
            // Auto-start if level data is assigned
            if (levelData != null && difficultySettings != null)
            {
                StartLevel(levelData, difficultySettings);
            }
        }
        
        private void Update()
        {
            if (!isPlaying || isPaused) return;
            
            // Handle pause input
            if (Input.GetKeyDown(pauseKey))
            {
                TogglePause();
                return;
            }
            
            // Update spawner
            noteSpawner.UpdateSpawner();
            
            // Update UI
            UpdateUI();
            
            // Check for hit input (left mouse button)
            if (Input.GetMouseButtonDown(0))
            {
                ProcessHitAttempt();
            }
            
            // Check if level is complete
            CheckLevelComplete();
        }
        
        /// <summary>
        /// Starts a level with the given data and difficulty
        /// </summary>
        public void StartLevel(LevelData level, DifficultySettings difficulty)
        {
            if (level == null || difficulty == null)
            {
                Debug.LogError("Cannot start level: Level data or difficulty is null!");
                return;
            }
            
            levelData = level;
            difficultySettings = difficulty;
            
            // Initialize systems
            timingSystem.Initialize(levelData, difficultySettings);
            scoringSystem.Initialize(difficultySettings);
            noteSpawner.Initialize(levelData, timingSystem);
            practiceManager.Initialize(timingSystem, audioSource, noteSpawner);
            
            // Setup audio
            audioSource.clip = levelData.song;
            audioSource.pitch = difficultySettings.speedMultiplier;
            
            // Setup UI
            gameplayUI.Initialize();
            gameplayUI.SetPracticeModeUI(practiceManager.IsPracticeMode);
            
            // Subscribe to events
            SubscribeToEvents();
            
            // Start timing and audio
            timingSystem.StartTiming();
            audioSource.Play();
            
            isPlaying = true;
            isPaused = false;
            levelComplete = false;
            
            Debug.Log($"Started level: {levelData.levelName} at {difficultySettings.difficultyName} difficulty ({difficultySettings.speedMultiplier}x speed)");
        }
        
        /// <summary>
        /// Processes a hit attempt from the player
        /// </summary>
        private void ProcessHitAttempt()
        {
            // Get the hovered hit object from cursor controller
            HitObject hoveredObject = cursorController.GetHoveredHitObject();
            
            if (hoveredObject == null || hoveredObject.IsHit || hoveredObject.IsMissed)
                return;
            
            // Check if the hit object is in the player zone
            if (!playerZone.IsOverlapping(hoveredObject))
                return;
            
            // Calculate timing error
            float currentTime = timingSystem.CurrentSongTime;
            float timingError = currentTime - hoveredObject.HitTime;
            
            // Evaluate the hit
            HitResult result = scoringSystem.EvaluateHit(timingError);
            
            // Register the hit
            scoringSystem.RegisterHit(result);
            
            // Mark the object as hit (or missed)
            hoveredObject.AttemptHit(currentTime);
            
            // Update UI
            gameplayUI.ShowHitResult(result);
            
            // Visual feedback
            if (result != HitResult.Miss)
            {
                playerZone.SetActive(true);
                Invoke(nameof(ResetPlayerZoneVisual), 0.1f);
            }
        }
        
        /// <summary>
        /// Updates the UI with current gameplay info
        /// </summary>
        private void UpdateUI()
        {
            gameplayUI.UpdateProgress(timingSystem.CurrentSongTime, levelData.GetSongDuration());
            gameplayUI.UpdateAccuracy(scoringSystem.Accuracy);
        }
        
        /// <summary>
        /// Checks if the level is complete
        /// </summary>
        private void CheckLevelComplete()
        {
            if (levelComplete) return;
            
            // Check if song has ended and all notes have been processed
            if (!audioSource.isPlaying && noteSpawner.NextHitObjectIndex >= levelData.hitObjects.Count)
            {
                // Give a grace period for last notes
                if (timingSystem.CurrentSongTime > audioSource.clip.length + 1f)
                {
                    CompletLevel();
                }
            }
        }
        
        /// <summary>
        /// Called when the level is completed
        /// </summary>
        private void CompletLevel()
        {
            levelComplete = true;
            isPlaying = false;
            
            // Show results
            ScoreData finalScore = scoringSystem.GetScoreData();
            resultsScreen.ShowResults(finalScore);
            
            Debug.Log($"Level Complete! Score: {finalScore.totalScore}, Accuracy: {finalScore.accuracy:F2}%");
        }
        
        /// <summary>
        /// Toggles pause state
        /// </summary>
        public void TogglePause()
        {
            isPaused = !isPaused;
            
            if (isPaused)
            {
                audioSource.Pause();
                Time.timeScale = 0f;
                gameplayUI.SetPauseMenu(true);
            }
            else
            {
                audioSource.UnPause();
                Time.timeScale = 1f;
                gameplayUI.SetPauseMenu(false);
            }
        }
        
        /// <summary>
        /// Restarts the current level
        /// </summary>
        public void RestartLevel()
        {
            // Stop current playback
            audioSource.Stop();
            noteSpawner.ClearAllHitObjects();
            
            // Reset pause state
            isPaused = false;
            Time.timeScale = 1f;
            
            // Restart
            StartLevel(levelData, difficultySettings);
        }
        
        /// <summary>
        /// Returns to main menu (placeholder)
        /// </summary>
        public void ReturnToMenu()
        {
            Time.timeScale = 1f;
            Debug.Log("Return to menu (implement scene loading here)");
            // UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }
        
        /// <summary>
        /// Subscribes to all relevant events
        /// </summary>
        private void SubscribeToEvents()
        {
            // Scoring events
            scoringSystem.OnScoreChanged += gameplayUI.UpdateScore;
            scoringSystem.OnComboChanged += gameplayUI.UpdateCombo;
            
            // Practice mode events
            if (practiceManager.IsPracticeMode)
            {
                practiceManager.OnCheckpointCreated += OnCheckpointCreated;
                practiceManager.OnCheckpointRemoved += OnCheckpointRemoved;
            }
            
            // Results screen buttons
            resultsScreen.SetupButtons(RestartLevel, ReturnToMenu);
        }
        
        /// <summary>
        /// Unsubscribes from all events
        /// </summary>
        private void UnsubscribeFromEvents()
        {
            if (scoringSystem != null)
            {
                scoringSystem.OnScoreChanged -= gameplayUI.UpdateScore;
                scoringSystem.OnComboChanged -= gameplayUI.UpdateCombo;
            }
            
            if (practiceManager != null)
            {
                practiceManager.OnCheckpointCreated -= OnCheckpointCreated;
                practiceManager.OnCheckpointRemoved -= OnCheckpointRemoved;
            }
        }
        
        /// <summary>
        /// Called when a checkpoint is created
        /// </summary>
        private void OnCheckpointCreated(CheckpointData checkpoint)
        {
            if (gameplayUI != null && gameplayUI.GetProgressBar() != null)
            {
                gameplayUI.GetProgressBar().AddCheckpointMarker(checkpoint, levelData.GetSongDuration());
            }
        }
        
        /// <summary>
        /// Called when a checkpoint is removed
        /// </summary>
        private void OnCheckpointRemoved(int index)
        {
            if (gameplayUI != null && gameplayUI.GetProgressBar() != null)
            {
                gameplayUI.GetProgressBar().RemoveLastCheckpointMarker();
            }
        }
        
        /// <summary>
        /// Resets the player zone visual state
        /// </summary>
        private void ResetPlayerZoneVisual()
        {
            playerZone.SetActive(false);
        }
        
        /// <summary>
        /// Validates that all required references are assigned
        /// </summary>
        private void ValidateReferences()
        {
            if (audioSource == null)
                Debug.LogError("AudioSource is not assigned!");
            if (noteSpawner == null)
                Debug.LogError("NoteSpawner is not assigned!");
            if (playerZone == null)
                Debug.LogError("PlayerZone is not assigned!");
            if (cursorController == null)
                Debug.LogError("CursorController is not assigned!");
            if (practiceManager == null)
                Debug.LogError("PracticeManager is not assigned!");
            if (gameplayUI == null)
                Debug.LogError("GameplayUI is not assigned!");
            if (resultsScreen == null)
                Debug.LogError("ResultsScreen is not assigned!");
        }
        
        private void OnDestroy()
        {
            UnsubscribeFromEvents();
            Time.timeScale = 1f; // Reset time scale on destroy
        }
    }
}
