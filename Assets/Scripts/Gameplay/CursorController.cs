using UnityEngine;
using System.Collections.Generic;

namespace Beatbreak.Gameplay
{
    /// <summary>
    /// Tracks the mouse cursor in world space and detects what it's hovering over
    /// </summary>
    public class CursorController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Camera mainCamera;
        
        private Vector2 cursorWorldPosition;
        private HitObject currentlyHoveredObject;
        private List<HitObject> allActiveHitObjects = new List<HitObject>();
        
        public Vector2 CursorWorldPosition => cursorWorldPosition;
        public HitObject HoveredObject => currentlyHoveredObject;
        
        private void Awake()
        {
            if (mainCamera == null)
                mainCamera = Camera.main;
        }
        
        private void Update()
        {
            UpdateCursorPosition();
            UpdateHoveredObject();
        }
        
        /// <summary>
        /// Updates the cursor position in world space
        /// </summary>
        private void UpdateCursorPosition()
        {
            Vector3 mousePosition = Input.mousePosition;
            cursorWorldPosition = mainCamera.ScreenToWorldPoint(mousePosition);
        }
        
        /// <summary>
        /// Updates which hit object is currently being hovered
        /// </summary>
        private void UpdateHoveredObject()
        {
            HitObject newHoveredObject = null;
            
            // Find the closest hit object under the cursor
            float closestDistance = float.MaxValue;
            
            foreach (HitObject hitObject in allActiveHitObjects)
            {
                if (hitObject == null || hitObject.IsHit || hitObject.IsMissed)
                    continue;
                
                if (hitObject.ContainsPoint(cursorWorldPosition))
                {
                    float distance = Vector2.Distance(cursorWorldPosition, hitObject.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        newHoveredObject = hitObject;
                    }
                }
            }
            
            // Update hover states
            if (currentlyHoveredObject != newHoveredObject)
            {
                if (currentlyHoveredObject != null)
                {
                    currentlyHoveredObject.SetHovered(false);
                }
                
                currentlyHoveredObject = newHoveredObject;
                
                if (currentlyHoveredObject != null)
                {
                    currentlyHoveredObject.SetHovered(true);
                }
            }
        }
        
        /// <summary>
        /// Registers a hit object for cursor detection
        /// </summary>
        public void RegisterHitObject(HitObject hitObject)
        {
            if (!allActiveHitObjects.Contains(hitObject))
            {
                allActiveHitObjects.Add(hitObject);
            }
        }
        
        /// <summary>
        /// Unregisters a hit object
        /// </summary>
        public void UnregisterHitObject(HitObject hitObject)
        {
            allActiveHitObjects.Remove(hitObject);
            
            if (currentlyHoveredObject == hitObject)
            {
                currentlyHoveredObject = null;
            }
        }
        
        /// <summary>
        /// Clears all registered hit objects
        /// </summary>
        public void ClearAllHitObjects()
        {
            allActiveHitObjects.Clear();
            currentlyHoveredObject = null;
        }
        
        /// <summary>
        /// Gets the currently hovered hit object
        /// </summary>
        public HitObject GetHoveredHitObject()
        {
            return currentlyHoveredObject;
        }
    }
}
