using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Beatbreak.Data;

namespace Beatbreak.UI
{
    /// <summary>
    /// Progress bar that displays song progress and checkpoints
    /// </summary>
    public class ProgressBarUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Image fillImage;
        [SerializeField] private RectTransform checkpointContainer;
        [SerializeField] private GameObject checkpointMarkerPrefab;
        
        [Header("Colors")]
        [SerializeField] private Color progressColor = Color.green;
        [SerializeField] private Color checkpointColor = Color.yellow;
        
        private List<GameObject> checkpointMarkers = new List<GameObject>();
        private RectTransform rectTransform;
        
        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }
        
        /// <summary>
        /// Updates the progress bar fill amount
        /// </summary>
        public void UpdateProgress(float currentTime, float totalTime)
        {
            if (fillImage != null && totalTime > 0)
            {
                float progress = Mathf.Clamp01(currentTime / totalTime);
                fillImage.fillAmount = progress;
            }
        }
        
        /// <summary>
        /// Adds a checkpoint marker to the progress bar
        /// </summary>
        public void AddCheckpointMarker(CheckpointData checkpoint, float songDuration)
        {
            if (checkpointMarkerPrefab == null || checkpointContainer == null)
                return;
            
            GameObject marker = Instantiate(checkpointMarkerPrefab, checkpointContainer);
            RectTransform markerRect = marker.GetComponent<RectTransform>();
            
            if (markerRect != null)
            {
                // Position the marker based on checkpoint time
                float normalizedTime = checkpoint.songTime / songDuration;
                float xPosition = normalizedTime * rectTransform.rect.width;
                markerRect.anchoredPosition = new Vector2(xPosition, 0);
                
                // Set color
                Image markerImage = marker.GetComponent<Image>();
                if (markerImage != null)
                {
                    markerImage.color = checkpointColor;
                }
            }
            
            checkpointMarkers.Add(marker);
        }
        
        /// <summary>
        /// Removes the last checkpoint marker
        /// </summary>
        public void RemoveLastCheckpointMarker()
        {
            if (checkpointMarkers.Count == 0) return;
            
            int lastIndex = checkpointMarkers.Count - 1;
            GameObject lastMarker = checkpointMarkers[lastIndex];
            checkpointMarkers.RemoveAt(lastIndex);
            Destroy(lastMarker);
        }
        
        /// <summary>
        /// Clears all checkpoint markers
        /// </summary>
        public void ClearAllCheckpointMarkers()
        {
            foreach (GameObject marker in checkpointMarkers)
            {
                if (marker != null)
                {
                    Destroy(marker);
                }
            }
            checkpointMarkers.Clear();
        }
        
        /// <summary>
        /// Updates checkpoint markers from a list
        /// </summary>
        public void UpdateCheckpointMarkers(List<CheckpointData> checkpoints, float songDuration)
        {
            ClearAllCheckpointMarkers();
            
            foreach (CheckpointData checkpoint in checkpoints)
            {
                AddCheckpointMarker(checkpoint, songDuration);
            }
        }
    }
}
