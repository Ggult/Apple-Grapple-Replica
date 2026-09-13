using System.Collections.Generic;
using UnityEngine;

namespace AppleGrapple
{
    public sealed class PickupRegistry : MonoBehaviour
    {
        public static PickupRegistry Instance { get; private set; }

        private readonly List<SwordPickup> _swordPickups = new();
        public IReadOnlyList<SwordPickup> SwordPickups => _swordPickups;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public void Register(Pickup pickup)
        {
            if (pickup is SwordPickup swordPickup && !_swordPickups.Contains(swordPickup))
                _swordPickups.Add(swordPickup);
        }

        public void Unregister(Pickup pickup)
        {
            if (pickup is SwordPickup swordPickup)
                _swordPickups.Remove(swordPickup);
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }
    }
}