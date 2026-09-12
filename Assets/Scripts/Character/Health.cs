using System;
using UnityEngine;
namespace AppleGrapple
{
    public class Health : MonoBehaviour
    {
        [SerializeField] private CharacterConfig _characterConfig;
        private int _currentHealth;

        public int CurrentHealth => _currentHealth;
        public int MaxHealth => _characterConfig.maxHealth;
        public float HealthPercent => MaxHealth <= 0 ? 0f : (float)_currentHealth / MaxHealth;
        public bool IsDead => _currentHealth <= 0;

        public event Action Died;
        public event Action<HitInfo> Damaged;
        public event Action<int, int> HealthChanged;

        private void Awake()
        {
            _currentHealth = MaxHealth;
        }

        public void TakeDamage(HitInfo hitInfo)
        {
            if (IsDead) return;

            _currentHealth -= hitInfo.DamageAmount;
            Damaged?.Invoke(hitInfo);
            HealthChanged?.Invoke(_currentHealth, MaxHealth);
            if (IsDead)
            {
                Debug.Log("Character died.");
                Died?.Invoke();
            }
        }
    }
}
