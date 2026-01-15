using UnityEngine;

namespace Beatbreak.Audio
{
    /// <summary>
    /// Manages audio playback with support for pitch/speed adjustments
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class AudioManager : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private AudioSource audioSource;
        
        [Header("Volume Settings")]
        [SerializeField] private float masterVolume = 1f;
        [SerializeField] private float musicVolume = 1f;
        [SerializeField] private float sfxVolume = 1f;
        
        private float currentSpeedMultiplier = 1f;
        
        public float MasterVolume => masterVolume;
        public float MusicVolume => musicVolume;
        public float SfxVolume => sfxVolume;
        public float CurrentSpeedMultiplier => currentSpeedMultiplier;
        
        private void Awake()
        {
            if (audioSource == null)
                audioSource = GetComponent<AudioSource>();
        }
        
        /// <summary>
        /// Plays an audio clip with the specified speed multiplier
        /// </summary>
        public void PlayMusic(AudioClip clip, float speedMultiplier = 1f)
        {
            if (clip == null)
            {
                Debug.LogError("Cannot play null audio clip!");
                return;
            }
            
            audioSource.clip = clip;
            audioSource.pitch = speedMultiplier;
            audioSource.volume = masterVolume * musicVolume;
            currentSpeedMultiplier = speedMultiplier;
            
            audioSource.Play();
        }
        
        /// <summary>
        /// Stops the current music
        /// </summary>
        public void StopMusic()
        {
            audioSource.Stop();
        }
        
        /// <summary>
        /// Pauses the current music
        /// </summary>
        public void PauseMusic()
        {
            audioSource.Pause();
        }
        
        /// <summary>
        /// Resumes the paused music
        /// </summary>
        public void ResumeMusic()
        {
            audioSource.UnPause();
        }
        
        /// <summary>
        /// Sets the music playback position
        /// </summary>
        public void SetMusicTime(float time)
        {
            audioSource.time = Mathf.Clamp(time, 0f, audioSource.clip.length);
        }
        
        /// <summary>
        /// Gets the current music playback position
        /// </summary>
        public float GetMusicTime()
        {
            return audioSource.time;
        }
        
        /// <summary>
        /// Checks if music is currently playing
        /// </summary>
        public bool IsPlaying()
        {
            return audioSource.isPlaying;
        }
        
        /// <summary>
        /// Sets the master volume
        /// </summary>
        public void SetMasterVolume(float volume)
        {
            masterVolume = Mathf.Clamp01(volume);
            UpdateVolume();
        }
        
        /// <summary>
        /// Sets the music volume
        /// </summary>
        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
            UpdateVolume();
        }
        
        /// <summary>
        /// Updates the audio source volume
        /// </summary>
        private void UpdateVolume()
        {
            audioSource.volume = masterVolume * musicVolume;
        }
        
        /// <summary>
        /// Changes the playback speed without stopping
        /// </summary>
        public void SetSpeedMultiplier(float multiplier)
        {
            currentSpeedMultiplier = Mathf.Clamp(multiplier, 0.25f, 3f);
            audioSource.pitch = currentSpeedMultiplier;
        }
    }
}
