using UnityEngine;
using Beatbreak.Data;
using Beatbreak.Core;

namespace Beatbreak.Gameplay
{
    /// <summary>
    /// Represents a single hit object (note) that moves toward the player zone
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class HitObject : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private BoxCollider2D boxCollider;
        
        [Header("Visual Feedback")]
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color hoverColor = Color.yellow;
        [SerializeField] private Color hitColor = Color.green;
        [SerializeField] private Color missColor = Color.red;
        
        // Hit object data
        private HitObjectData data;
        private Vector2 startPosition;
        private Vector2 targetPosition;
        private float spawnTime;
        private float hitTime;
        private float approachDuration;
        private bool isHit;
        private bool isMissed;
        private bool isHovered;
        
        // Events
        public event System.Action<HitObject, float> OnHitAttempt;
        public event System.Action<HitObject> OnMissed;
        
        // Properties
        public float HitTime => hitTime;
        public bool IsHit => isHit;
        public bool IsMissed => isMissed;
        public bool IsHovered => isHovered;
        public HitObjectData Data => data;
        
        private void Awake()
        {
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
            if (boxCollider == null)
                boxCollider = GetComponent<BoxCollider2D>();
        }
        
        /// <summary>
        /// Initializes the hit object with its data
        /// </summary>
        public void Initialize(HitObjectData objectData, Vector2 spawnPos, Vector2 targetPos, 
            float objectHitTime, float currentTime, float approachTime)
        {
            data = objectData;
            startPosition = spawnPos;
            targetPosition = targetPos;
            hitTime = objectHitTime;
            spawnTime = currentTime;
            approachDuration = approachTime;
            isHit = false;
            isMissed = false;
            isHovered = false;
            
            // Set initial position
            transform.position = startPosition;
            
            // Set size
            transform.localScale = Vector3.one * objectData.sizeMultiplier;
            
            // Set initial color
            spriteRenderer.color = normalColor;
        }
        
        /// <summary>
        /// Updates the hit object position based on time
        /// </summary>
        public void UpdatePosition(float currentTime)
        {
            if (isHit || isMissed) return;
            
            // Calculate progress (0 to 1)
            float timeSinceSpawn = currentTime - spawnTime;
            float progress = Mathf.Clamp01(timeSinceSpawn / approachDuration);
            
            // Apply easing (can be customized)
            float easedProgress = EaseOutCubic(progress);
            
            // Lerp position
            transform.position = Vector2.Lerp(startPosition, targetPosition, easedProgress);
            
            // Check if we've passed the hit time (auto-miss)
            float timingError = currentTime - hitTime;
            if (timingError > 0.15f && !isHit) // Grace period after hit time
            {
                Miss();
            }
        }
        
        /// <summary>
        /// Called when cursor enters this hit object
        /// </summary>
        public void SetHovered(bool hovered)
        {
            if (isHit || isMissed) return;
            
            isHovered = hovered;
            spriteRenderer.color = hovered ? hoverColor : normalColor;
        }
        
        /// <summary>
        /// Attempts to hit this object
        /// </summary>
        public void AttemptHit(float currentTime)
        {
            if (isHit || isMissed) return;
            
            float timingError = currentTime - hitTime;
            
            isHit = true;
            spriteRenderer.color = hitColor;
            
            OnHitAttempt?.Invoke(this, timingError);
            
            // Destroy after a short delay for visual feedback
            Destroy(gameObject, 0.1f);
        }
        
        /// <summary>
        /// Marks this object as missed
        /// </summary>
        public void Miss()
        {
            if (isHit || isMissed) return;
            
            isMissed = true;
            spriteRenderer.color = missColor;
            
            OnMissed?.Invoke(this);
            
            // Fade out and destroy
            Destroy(gameObject, 0.3f);
        }
        
        /// <summary>
        /// Checks if the hit object is in the hit zone
        /// </summary>
        public bool IsInHitZone(Bounds hitZoneBounds)
        {
            return hitZoneBounds.Intersects(boxCollider.bounds);
        }
        
        /// <summary>
        /// Checks if a point is inside this hit object
        /// </summary>
        public bool ContainsPoint(Vector2 point)
        {
            return boxCollider.bounds.Contains(point);
        }
        
        // Easing function for smooth movement
        private float EaseOutCubic(float t)
        {
            return 1f - Mathf.Pow(1f - t, 3f);
        }
        
        private void OnDestroy()
        {
            OnHitAttempt = null;
            OnMissed = null;
        }
    }
}
