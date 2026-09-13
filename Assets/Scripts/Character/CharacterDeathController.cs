using System;
using UnityEngine;

namespace AppleGrapple
{
    public class CharacterDeathController : MonoBehaviour
    {
        private CharacterRoot _character;
        private Health _health;
        private SwordOrigin _swordOrigin;
        private Collider2D[] _colliders;
        private bool _hasDied;

        public event Action<CharacterDeathController> Died;
        public bool IsDead => _hasDied;
        public bool IsPlayer => _character.Identity.IsPlayer;

        private void Awake()
        {
            _character = GetComponent<CharacterRoot>();
            _health = _character.Health;
            _swordOrigin = _character.SwordOrigin;
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

            Died?.Invoke(this);
        }

        public void StopMovement()
        {
            _character.Rigidbody.linearVelocity = Vector2.zero;
            _character.Rigidbody.angularVelocity = 0f;

            _character.Movement.ClearInputProvider();
            _character.Movement.enabled = false;
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
