using System.Collections;
using UnityEngine;
namespace AppleGrapple
{
    // Stand-in target with no decision making: same Health/SwordOrigin a real character would use.
    // Swap this out for an AI move/decision component later without touching combat code.
    [RequireComponent(typeof(Health), typeof(SwordOrigin), typeof(Rigidbody2D))]
    public class DummyEnemy : MonoBehaviour
    {
        [SerializeField] private CharacterConfig _characterConfig;
        [SerializeField] private int _startingSwordCount = 1;

        private SwordOrigin _swordOrigin;
        private Health _health;
        private Rigidbody2D _rb;
        private Coroutine _knockbackRoutine;

        private void Awake()
        {
            _swordOrigin = GetComponent<SwordOrigin>();
            _health = GetComponent<Health>();
            _rb = GetComponent<Rigidbody2D>();
            _health.Died += HandleDied;
            _health.Damaged += HandleDamaged;
        }

        private void Start()
        {
            for (int i = 0; i < _startingSwordCount; i++)
            {
                _swordOrigin.AddWeapon();
            }
        }

        private void HandleDied()
        {
            Debug.Log($"{name} died.");
        }

        // No movement controller to blend into here, so drive the Rigidbody2D directly with a decaying push.
        private void HandleDamaged(HitInfo hitInfo)
        {
            var direction = ((Vector2)transform.position - (Vector2)hitInfo.Position).normalized;

            if (_knockbackRoutine != null)
            {
                StopCoroutine(_knockbackRoutine);
            }
            _knockbackRoutine = StartCoroutine(KnockbackRoutine(direction * _characterConfig.knockbackForce));
        }

        private IEnumerator KnockbackRoutine(Vector2 initialVelocity)
        {
            float elapsed = 0f;
            while (elapsed < _characterConfig.knockbackDuration)
            {
                _rb.linearVelocity = Vector2.Lerp(initialVelocity, Vector2.zero, elapsed / _characterConfig.knockbackDuration);
                elapsed += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
            _rb.linearVelocity = Vector2.zero;
            _knockbackRoutine = null;
        }
    }
}
