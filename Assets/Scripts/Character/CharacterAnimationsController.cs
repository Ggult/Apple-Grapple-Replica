using UnityEngine;

namespace AppleGrapple
{
    [RequireComponent(typeof(CharacterRoot))]
    public class CharacterAnimationsController : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private SpriteRenderer _body;
        [SerializeField] private bool _flipWithMovement = true;

        private CharacterRoot _character;
        private const string SpeedParameterId = "Speed";
        private const string DeadTriggerId = "OnDead";
        private bool _isDead;

        private void Awake()
        {
            _character = GetComponent<CharacterRoot>();
            _animator ??= GetComponent<Animator>();
            _body ??= GetComponentInChildren<SpriteRenderer>();
            _character.Health.Died += OnDead;
        }

        private void OnDestroy()
        {
            if (_character != null && _character.Health != null)
                _character.Health.Died -= OnDead;
        }

        private void Update()
        {
            if (_isDead) return;
            var velocity = _character.Rigidbody.linearVelocity;
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
