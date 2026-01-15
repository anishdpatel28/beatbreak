using UnityEngine;
using System.Collections.Generic;
using Beatbreak.Data;
using Beatbreak.Core;

namespace Beatbreak.Gameplay
{
    /// <summary>
    /// Spawns and manages hit objects based on the level data and timing
    /// </summary>
    public class NoteSpawner : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject hitObjectPrefab;
        [SerializeField] private Transform hitObjectContainer;
        [SerializeField] private PlayerZone playerZone;
        [SerializeField] private CursorController cursorController;
        
        [Header("Spawn Settings")]
        [SerializeField] private float spawnDistance = 10f;
        
        private LevelData currentLevel;
        private TimingSystem timingSystem;
        private List<HitObject> activeHitObjects = new List<HitObject>();
        private int nextHitObjectIndex;
        private float approachTime;
        
        public List<HitObject> ActiveHitObjects => activeHitObjects;
        public int NextHitObjectIndex => nextHitObjectIndex;
        
        /// <summary>
        /// Initializes the spawner with level data and timing system
        /// </summary>
        public void Initialize(LevelData levelData, TimingSystem timing)
        {
            currentLevel = levelData;
            timingSystem = timing;
            approachTime = levelData.defaultApproachTime / timingSystem.SpeedMultiplier;
            nextHitObjectIndex = 0;
            
            ClearAllHitObjects();
        }
        
        /// <summary>
        /// Updates the spawner, spawning notes when needed
        /// </summary>
        public void UpdateSpawner()
        {
            if (currentLevel == null || timingSystem == null)
                return;
            
            // Check if we should spawn the next hit object
            while (nextHitObjectIndex < currentLevel.hitObjects.Count)
            {
                HitObjectData nextObject = currentLevel.hitObjects[nextHitObjectIndex];
                float hitTime = timingSystem.BeatsToSeconds(nextObject.beatTime);
                float spawnTime = hitTime - approachTime;
                
                if (timingSystem.CurrentSongTime >= spawnTime)
                {
                    SpawnHitObject(nextObject, hitTime);
                    nextHitObjectIndex++;
                }
                else
                {
                    break; // Wait for the next frame
                }
            }
            
            // Update all active hit objects
            UpdateActiveHitObjects();
        }
        
        /// <summary>
        /// Spawns a single hit object
        /// </summary>
        private void SpawnHitObject(HitObjectData objectData, float hitTime)
        {
            if (hitObjectPrefab == null)
            {
                Debug.LogError("HitObject prefab is not assigned!");
                return;
            }
            
            // Calculate spawn position based on approach direction
            Vector2 approachDir = objectData.GetApproachDirection();
            Vector2 spawnPosition = (Vector2)playerZone.transform.position + (approachDir * spawnDistance * objectData.speedMultiplier);
            spawnPosition += objectData.positionOffset;
            
            // Instantiate the hit object
            GameObject hitObjectGO = Instantiate(hitObjectPrefab, spawnPosition, Quaternion.identity, hitObjectContainer);
            HitObject hitObject = hitObjectGO.GetComponent<HitObject>();
            
            if (hitObject != null)
            {
                // Initialize the hit object
                hitObject.Initialize(
                    objectData,
                    spawnPosition,
                    playerZone.transform.position,
                    hitTime,
                    timingSystem.CurrentSongTime,
                    approachTime / objectData.speedMultiplier
                );
                
                // Register with cursor controller
                cursorController.RegisterHitObject(hitObject);
                
                // Add to active list
                activeHitObjects.Add(hitObject);
            }
        }
        
        /// <summary>
        /// Updates all active hit objects
        /// </summary>
        private void UpdateActiveHitObjects()
        {
            for (int i = activeHitObjects.Count - 1; i >= 0; i--)
            {
                HitObject hitObject = activeHitObjects[i];
                
                if (hitObject == null)
                {
                    activeHitObjects.RemoveAt(i);
                    continue;
                }
                
                if (!hitObject.IsHit && !hitObject.IsMissed)
                {
                    hitObject.UpdatePosition(timingSystem.CurrentSongTime);
                }
            }
        }
        
        /// <summary>
        /// Clears all active hit objects
        /// </summary>
        public void ClearAllHitObjects()
        {
            foreach (HitObject hitObject in activeHitObjects)
            {
                if (hitObject != null)
                {
                    cursorController.UnregisterHitObject(hitObject);
                    Destroy(hitObject.gameObject);
                }
            }
            
            activeHitObjects.Clear();
        }
        
        /// <summary>
        /// Resets the spawner to a specific index (for checkpoints)
        /// </summary>
        public void ResetToIndex(int index)
        {
            ClearAllHitObjects();
            nextHitObjectIndex = Mathf.Clamp(index, 0, currentLevel.hitObjects.Count);
        }
        
        /// <summary>
        /// Gets the next hit object index for checkpoint creation
        /// </summary>
        public int GetNextSpawnIndex()
        {
            return nextHitObjectIndex;
        }
    }
}
