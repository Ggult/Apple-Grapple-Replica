using System;
using UnityEngine;

namespace AppleGrapple
{
    [RequireComponent(typeof(Health), typeof(SwordOrigin), typeof(Rigidbody2D))]
    public class CharacterDeathController : MonoBehaviour
    {
        private Health _health;
        private SwordOrigin _swordOrigin;
        private Rigidbody2D _rigidbody;
        private CharacterView _characterView;
        private EnemyStateMachine _enemyStateMachine;
        private Collider2D[] _colliders;
        private bool _hasDied;

        public event Action<CharacterDeathController> Died;
        public bool IsDead => _hasDied;
        public bool IsPlayer => GetComponent<CharacterIdentity>()?.IsPlayer ?? false;

        private void Awake()
        {
            _health = GetComponent<Health>();
            _swordOrigin = GetComponent<SwordOrigin>();
            _rigidbody = GetComponent<Rigidbody2D>();
            _characterView = GetComponent<CharacterView>();
            _enemyStateMachine = GetComponent<EnemyStateMachine>();
            _colliders = GetComponentsInChildren<Collider2D>(true);
            _health.Died += HandleDied;
        }

        private void OnDestroy()
        {
            if (_health != null)
            {
                _health.Died -= HandleDied;
            }
        }

        private void HandleDied()
        {
            if (_hasDied)
                return;

            _hasDied = true;
            StopMovement();
            DisableColliders();
            _swordOrigin.RemoveAllWeapons();
            _characterView?.RemoveCharacterInfo();

            if (_enemyStateMachine != null && _enemyStateMachine.DeadState != null)
            {
                _enemyStateMachine.ChangeState(_enemyStateMachine.DeadState);
            }

            Died?.Invoke(this);
        }

        private void StopMovement()
        {
            _rigidbody.linearVelocity = Vector2.zero;
            _rigidbody.angularVelocity = 0f;

            var movementController = GetComponent<CharacterMovementController>();
            if (movementController != null)
            {
                movementController.enabled = false;
            }
        }

        private void DisableColliders()
        {
            foreach (var collider in _colliders)
            {
                collider.enabled = false;
            }
        }
    }
}
