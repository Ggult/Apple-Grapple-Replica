using UnityEngine;

namespace AppleGrapple
{
    [RequireComponent(typeof(CharacterMovementController))]
    public class PlayerMovementController : MonoBehaviour
    {
        private CharacterMovementController _movementController;

        private void Awake()
        {
            _movementController = GetComponent<CharacterMovementController>();
        }

        private void Start()
        {
            _movementController.SetInputProvider(new MovementInputProvider());
        }
    }
}
