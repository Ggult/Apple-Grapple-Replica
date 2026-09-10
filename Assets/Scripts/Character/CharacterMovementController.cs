using UnityEngine;
namespace AppleGrapple
{
    public class CharacterMovementController : MonoBehaviour
    {
        [SerializeField] private float _movementSpeed = 5f;
        private IInputProvider _inputProvider;
        private Rigidbody2D _rb;
        private Animator _animator;
        [SerializeField] private SpriteRenderer body;
        private const string SpeedParameter = "Speed";
        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();      
            _animator = GetComponent<Animator>();
            _inputProvider = new MovementInputProvider();
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

        public void Move(Vector2 direction)
        {
            if (_rb != null)
            {
                _rb.linearVelocity = direction * _movementSpeed;
            }
            if (body != null && _inputProvider != null && _inputProvider.IsDragging)
            {
                body.flipX = direction.x < 0;
            }
            if (_animator != null)
            {
                _animator.SetFloat(SpeedParameter, direction.magnitude);
            }
        }
    }
}
