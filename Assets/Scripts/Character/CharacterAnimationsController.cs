using UnityEngine;

namespace AppleGrapple
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Health))]
    public class CharacterAnimationsController : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private SpriteRenderer _body;
        [SerializeField] private bool _flipWithMovement = true;

        private Rigidbody2D _rigidbody;
        private Health _health;
        private const string SpeedParameterId = "Speed";
        private const string DeadTriggerId = "OnDead";
        private bool _isDead;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _health = GetComponent<Health>();
            _animator ??= GetComponent<Animator>();
            _body ??= GetComponentInChildren<SpriteRenderer>();
            _health.Died += OnDead;
        }

        private void OnDestroy()
        {
            if (_health != null)
            {
                _health.Died -= OnDead;
            }
        }

        private void Update()
        {
            if (_isDead) return;
            var velocity = _rigidbody.linearVelocity;
            var speed = Mathf.Clamp01(velocity.magnitude);

            if (_animator != null)
            {
                _animator.SetFloat(SpeedParameterId, speed);
            }

            if (_flipWithMovement && _body != null && Mathf.Abs(velocity.x) > 0.01f)
            {
                _body.flipX = velocity.x < 0f;
            }
        }

        private void OnDead()
        {
            if (_isDead) return;
            _isDead = true;

            if (_animator != null)
            {
                _animator.SetTrigger(DeadTriggerId);
            }
        }
    }
}
