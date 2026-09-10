using UnityEngine;
using UnityEngine.InputSystem;

namespace AppleGrapple
{
    public class MovementInputProvider : IInputProvider
    {
        private const float joystickRadiusPixels = 100f;
        private const float minDragDistancePixels = 10f;
        public Vector2 GetMovementInput()
        {
            return currentInput;
        }
        private Vector2 dragStartScreenPosition;
        private Vector2 currentInput;
        private bool _isDragging;
        private bool hasPressStart;
        public bool IsDragging => _isDragging;

        public void UpdateInput()
        {
            var isPressed = TryGetPointerPosition(out var screenPosition);

            if (isPressed && !hasPressStart)
            {
                dragStartScreenPosition = screenPosition;
                hasPressStart = true;
            }

            if (isPressed && hasPressStart && !_isDragging)
            {
                var dragDistance = Vector2.Distance(screenPosition, dragStartScreenPosition);
                _isDragging = dragDistance >= minDragDistancePixels;
            }

            if (!isPressed && hasPressStart)
            {
                _isDragging = false;
                currentInput = Vector2.zero;
                dragStartScreenPosition = Vector2.zero;
                hasPressStart = false;
            }

            if (_isDragging)
            {
                var joystickOffset = screenPosition - dragStartScreenPosition;
                var joystickStrength = Mathf.Clamp01(joystickOffset.magnitude / joystickRadiusPixels);
                currentInput = joystickOffset.sqrMagnitude > Mathf.Epsilon
                    ? joystickOffset.normalized * joystickStrength
                    : Vector2.zero;
            }
        }

         private static bool TryGetPointerPosition(out Vector2 screenPosition)
        {
            if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
            {
                screenPosition = Touchscreen.current.primaryTouch.position.ReadValue();
                return true;
            }

            if (Mouse.current != null && Mouse.current.leftButton.isPressed)
            {
                screenPosition = Mouse.current.position.ReadValue();
                return true;
            }

            screenPosition = Vector2.zero;
            return false;
        }

    }
}
