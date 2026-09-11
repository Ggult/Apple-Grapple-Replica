using System;
using UnityEngine;
namespace AppleGrapple
{
    public class Health : MonoBehaviour
    {
        [SerializeField] private int _maxHealth = 3;
        private int _currentHealth;

        public int CurrentHealth => _currentHealth;
        public int MaxHealth => _maxHealth;
        public bool IsDead => _currentHealth <= 0;

        public event Action Died;
        public event Action<Vector2> Damaged;
        public event Action<int, int> HealthChanged;

        private void Awake()
        {
            _currentHealth = _maxHealth;
        }

        public void TakeDamage(int amount, Vector2 sourcePosition)
        {
            if (IsDead) return;

            _currentHealth -= amount;
            Damaged?.Invoke(sourcePosition);
            HealthChanged?.Invoke(_currentHealth, _maxHealth);
            if (IsDead)
            {
                Debug.Log("Character died.");
                Died?.Invoke();
            }
        }
    }
}
