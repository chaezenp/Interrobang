using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class MobileTouchManager : MonoBehaviour
{
    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    void Update()
    {
        var activeTouches = Touch.activeTouches;

        if (activeTouches.Count > 0)
        {
            // Track finger on the screen
            Touch primaryTouch = activeTouches[0];

            if (primaryTouch.began)
            {
                Debug.Log($"Finger hit screen at: {primaryTouch.startScreenPosition}");
            }
            
            if (primaryTouch.isInProgress)
            {
                Vector2 currentPosition = primaryTouch.screenPosition;
                Vector2 dragDelta = primaryTouch.delta;
                Debug.Log($"Dragging finger. Frame delta: {dragDelta}");
            }

            if (primaryTouch.ended)
            {
                Debug.Log("Finger lifted off screen.");
            }
        }
    }

}
