using System;
using UnityEngine;
namespace AppleGrapple
{
    public class Health : MonoBehaviour
    {
        private CharacterRoot _character;
        private int _currentHealth;

        public int CurrentHealth => _currentHealth;
        public int MaxHealth => _character.CharacterConfig.maxHealth;
        public float HealthPercent => MaxHealth <= 0 ? 0f : (float)_currentHealth / MaxHealth;
        public bool IsDead => _currentHealth <= 0;

        public event Action Died;
        public event Action<HitInfo> Damaged;
        public event Action<int, int> HealthChanged;

        private void Awake()
        {
            _character = GetComponent<CharacterRoot>();
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
