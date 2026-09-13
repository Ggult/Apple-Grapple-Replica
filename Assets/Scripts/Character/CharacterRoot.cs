using System;
using UnityEngine;

namespace AppleGrapple
{
    [RequireComponent(typeof(Health))]
    [RequireComponent(typeof(CharacterIdentity))]
    [RequireComponent(typeof(CharacterMovementController))]
    [RequireComponent(typeof(SwordOrigin))]
    [RequireComponent(typeof(CharacterDeathController))]
    [RequireComponent(typeof(Rigidbody2D))]
    [DefaultExecutionOrder(-100)]
    public sealed class CharacterRoot : MonoBehaviour
    {
        [SerializeField] private CharacterConfig _characterConfig;

        public CharacterConfig CharacterConfig => _characterConfig;
        public Health Health { get; private set; }
        public CharacterIdentity Identity { get; private set; }
        public Rigidbody2D Rigidbody { get; private set; }
        public CharacterMovementController Movement { get; private set; }
        public SwordOrigin SwordOrigin { get; private set; }
        public CharacterDeathController Death { get; private set; }

        public bool IsPlayer => Identity.IsPlayer;
        public bool IsDead => Death.IsDead;
        public event Action<CharacterRoot> Died;

        private void Awake()
        {
            Health = GetComponent<Health>();
            Identity = GetComponent<CharacterIdentity>();
            Rigidbody = GetComponent<Rigidbody2D>();
            Movement = GetComponent<CharacterMovementController>();
            SwordOrigin = GetComponent<SwordOrigin>();
            Death = GetComponent<CharacterDeathController>();
            Death.Died += HandleDied;
        }

        private void OnEnable()
        {
            CharacterRegistry.Instance?.Register(this);
        }

        private void OnDestroy()
        {
            if (Death != null)
                Death.Died -= HandleDied;
        }

            private void OnDisable()
            {
                CharacterRegistry.Instance?.Unregister(this);
            }

        public void StopMovement()
        {
            Death.StopMovement();
        }

        private void HandleDied(CharacterDeathController deathController)
        {
            Died?.Invoke(this);
        }
    }
}