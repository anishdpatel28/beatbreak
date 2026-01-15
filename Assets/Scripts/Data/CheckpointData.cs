using UnityEngine;

namespace Beatbreak.Data
{
    /// <summary>
    /// Represents a checkpoint in practice mode
    /// </summary>
    [System.Serializable]
    public class CheckpointData
    {
        [Tooltip("Song time in seconds where this checkpoint is placed")]
        public float songTime;
        
        [Tooltip("Index of the first hit object to spawn after this checkpoint")]
        public int hitObjectIndex;
        
        [Tooltip("Display name for this checkpoint")]
        public string checkpointName;
        
        public CheckpointData(float time, int index)
        {
            songTime = time;
            hitObjectIndex = index;
            checkpointName = $"Checkpoint {FormatTime(time)}";
        }
        
        private string FormatTime(float time)
        {
            int minutes = Mathf.FloorToInt(time / 60f);
            int seconds = Mathf.FloorToInt(time % 60f);
            int milliseconds = Mathf.FloorToInt((time * 100f) % 100f);
            return $"{minutes:00}:{seconds:00}.{milliseconds:00}";
        }
    }
}
