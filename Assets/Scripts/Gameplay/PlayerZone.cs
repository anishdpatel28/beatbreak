using UnityEngine;

namespace Beatbreak.Gameplay
{
    /// <summary>
    /// Represents the central player zone (hit area) where notes must be hit
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class PlayerZone : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private BoxCollider2D boxCollider;
        
        [Header("Visual Settings")]
        [SerializeField] private Color normalColor = new Color(1f, 1f, 1f, 0.3f);
        [SerializeField] private Color activeColor = new Color(0f, 1f, 0f, 0.5f);
        
        [Header("Zone Settings")]
        [SerializeField] private Vector2 zoneSize = new Vector2(2f, 2f);
        
        // Boss mechanic support
        private Vector2 originalSize;
        private Vector2 targetSize;
        private float sizeTransitionSpeed = 5f;
        
        public Bounds Bounds => boxCollider.bounds;
        public Vector2 ZoneSize => zoneSize;
        
        private void Awake()
        {
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
            if (boxCollider == null)
                boxCollider = GetComponent<BoxCollider2D>();
                
            Initialize();
        }
        
        /// <summary>
        /// Initializes the player zone
        /// </summary>
        public void Initialize()
        {
            originalSize = zoneSize;
            targetSize = zoneSize;
            
            // Set the sprite renderer size
            transform.localScale = new Vector3(zoneSize.x, zoneSize.y, 1f);
            
            // Set the box collider size
            boxCollider.size = Vector2.one; // Scale handles the actual size
            
            spriteRenderer.color = normalColor;
        }
        
        private void Update()
        {
            // Smooth size transitions (for boss mechanics)
            if (Vector2.Distance(transform.localScale, targetSize) > 0.01f)
            {
                Vector2 currentSize = transform.localScale;
                Vector2 newSize = Vector2.Lerp(currentSize, targetSize, Time.deltaTime * sizeTransitionSpeed);
                transform.localScale = new Vector3(newSize.x, newSize.y, 1f);
            }
        }
        
        /// <summary>
        /// Sets the visual state of the zone (for feedback)
        /// </summary>
        public void SetActive(bool active)
        {
            spriteRenderer.color = active ? activeColor : normalColor;
        }
        
        /// <summary>
        /// Changes the zone size (for boss mechanics)
        /// </summary>
        public void SetZoneSize(Vector2 newSize, bool instant = false)
        {
            targetSize = newSize;
            
            if (instant)
            {
                transform.localScale = new Vector3(newSize.x, newSize.y, 1f);
            }
        }
        
        /// <summary>
        /// Resets the zone size to original
        /// </summary>
        public void ResetZoneSize()
        {
            SetZoneSize(originalSize);
        }
        
        /// <summary>
        /// Checks if a point is inside the player zone
        /// </summary>
        public bool ContainsPoint(Vector2 point)
        {
            return boxCollider.bounds.Contains(point);
        }
        
        /// <summary>
        /// Checks if a hit object is overlapping the zone
        /// </summary>
        public bool IsOverlapping(HitObject hitObject)
        {
            return hitObject.IsInHitZone(boxCollider.bounds);
        }
    }
}
