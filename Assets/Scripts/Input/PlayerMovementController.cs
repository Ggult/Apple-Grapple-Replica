using UnityEngine;

namespace AppleGrapple
{
    [RequireComponent(typeof(CharacterRoot))]
    public class PlayerMovementController : MonoBehaviour
    {
        private CharacterRoot _character;

        private void Awake()
        {
            _character = GetComponent<CharacterRoot>();
        }

        private void Start()
        {
            _character.Movement.SetInputProvider(new MovementInputProvider());
        }
    }
}
