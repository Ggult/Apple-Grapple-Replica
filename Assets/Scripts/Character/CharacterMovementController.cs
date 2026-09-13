using UnityEngine;
namespace AppleGrapple
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class CharacterMovementController : MonoBehaviour
    {
        private IInputProvider _inputProvider;
        private CharacterRoot _character;

        private Vector2 _externalVelocityStart;
        private float _externalVelocityDuration;
        private float _externalVelocityElapsed;

        private void Awake()
        {
            _character = GetComponent<CharacterRoot>();

            _character.Health.Damaged += HandleDamaged;
        }

        private void OnDestroy()
        {
            if (_character != null && _character.Health != null)
                _character.Health.Damaged -= HandleDamaged;
        }

        public void SetInputProvider(IInputProvider inputProvider)
        {
            _inputProvider = inputProvider;
        }

        public void ClearInputProvider()
        {
            _inputProvider = null;

            _character.Rigidbody.linearVelocity = Vector2.zero;
        }

        private void Update()
        {
            if (_inputProvider != null)
            {
                _inputProvider.UpdateInput();
            }
        }
        private void FixedUpdate()
        {
            if (_inputProvider != null)
            {
                Move(_inputProvider.GetMovementInput());
            }
        }

        // Adds a decaying push on top of input velocity instead of locking movement, so the player can still steer/escape while shoved.
        private void HandleDamaged(HitInfo hitInfo)
        {
            var direction = ((Vector2)transform.position - (Vector2)hitInfo.Position).normalized;
            _externalVelocityStart = direction * _character.CharacterConfig.knockbackForce;
            _externalVelocityDuration = _character.CharacterConfig.knockbackDuration;
            _externalVelocityElapsed = 0f;
        }

        private Vector2 GetExternalVelocity()
        {
            if (_externalVelocityDuration <= 0f) return Vector2.zero;

            _externalVelocityElapsed += Time.fixedDeltaTime;
            var t = Mathf.Clamp01(_externalVelocityElapsed / _externalVelocityDuration);
            if (t >= 1f) _externalVelocityDuration = 0f;

            return Vector2.Lerp(_externalVelocityStart, Vector2.zero, t);
        }

        public void Move(Vector2 direction)
        {
            _character.Rigidbody.linearVelocity = direction * _character.CharacterConfig.movementSpeed + GetExternalVelocity();
        }
    }
}
