using UnityEngine;
using Beatbreak.Data;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Beatbreak.Utilities
{
    /// <summary>
    /// Editor helper for creating and testing levels
    /// </summary>
    [ExecuteInEditMode]
    public class LevelEditorHelper : MonoBehaviour
    {
        [Header("Level Creation")]
        [SerializeField] private LevelData levelData;
        [SerializeField] private float currentBeat;
        [SerializeField] private float approachAngle;
        [SerializeField] private float speedMultiplier = 1f;
        [SerializeField] private float sizeMultiplier = 1f;
        
        [Header("Preview")]
        [SerializeField] private bool showPreview = true;
        [SerializeField] private Color previewColor = Color.cyan;
        [SerializeField] private float previewDistance = 10f;
        
#if UNITY_EDITOR
        /// <summary>
        /// Adds a hit object to the level at the current beat
        /// </summary>
        [ContextMenu("Add Hit Object at Current Beat")]
        public void AddHitObject()
        {
            if (levelData == null)
            {
                Debug.LogError("Level Data is not assigned!");
                return;
            }
            
            HitObjectData newHitObject = new HitObjectData
            {
                beatTime = currentBeat,
                approachAngle = approachAngle,
                speedMultiplier = speedMultiplier,
                sizeMultiplier = sizeMultiplier
            };
            
            levelData.hitObjects.Add(newHitObject);
            EditorUtility.SetDirty(levelData);
            
            Debug.Log($"Added hit object at beat {currentBeat}");
        }
        
        /// <summary>
        /// Removes the last hit object
        /// </summary>
        [ContextMenu("Remove Last Hit Object")]
        public void RemoveLastHitObject()
        {
            if (levelData == null || levelData.hitObjects.Count == 0)
                return;
            
            levelData.hitObjects.RemoveAt(levelData.hitObjects.Count - 1);
            EditorUtility.SetDirty(levelData);
            
            Debug.Log("Removed last hit object");
        }
        
        /// <summary>
        /// Sorts hit objects by beat time
        /// </summary>
        [ContextMenu("Sort Hit Objects by Beat")]
        public void SortHitObjects()
        {
            if (levelData == null)
                return;
            
            levelData.hitObjects.Sort((a, b) => a.beatTime.CompareTo(b.beatTime));
            EditorUtility.SetDirty(levelData);
            
            Debug.Log("Sorted hit objects by beat time");
        }
        
        /// <summary>
        /// Calculates the time in seconds for the current beat
        /// </summary>
        [ContextMenu("Calculate Time for Current Beat")]
        public void CalculateTime()
        {
            if (levelData == null)
                return;
            
            float timeInSeconds = currentBeat * (60f / levelData.bpm);
            Debug.Log($"Beat {currentBeat} = {timeInSeconds:F3} seconds at {levelData.bpm} BPM");
        }
        
        private void OnDrawGizmos()
        {
            if (!showPreview || levelData == null)
                return;
            
            // Draw preview of where the note would spawn
            Vector3 center = transform.position;
            float radians = approachAngle * Mathf.Deg2Rad;
            Vector3 direction = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0);
            Vector3 spawnPosition = center + direction * previewDistance;
            
            Gizmos.color = previewColor;
            Gizmos.DrawLine(center, spawnPosition);
            Gizmos.DrawWireSphere(spawnPosition, 0.5f * sizeMultiplier);
            
            // Draw angle indicator
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(center, 1f);
        }
#endif
    }
}
