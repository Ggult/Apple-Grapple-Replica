using UnityEngine;
namespace AppleGrapple
{
    public interface IInputProvider
    {
        public Vector2 GetMovementInput();
        public void UpdateInput();
        public bool IsDragging { get; }
    }
}
