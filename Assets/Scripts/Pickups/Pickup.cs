using System;
using UnityEngine;
namespace AppleGrapple
{
    // Base for ground pickups: spawner owns pooling, subclasses only decide what collecting does.
    [RequireComponent(typeof(Collider2D))]
    public abstract class Pickup : MonoBehaviour
    {
        public event Action<Pickup> Collected;

        private void Awake()
        {
            GetComponent<Collider2D>().isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var collector = other.GetComponentInParent<PickupCollector>();
            if (collector == null) return;

            if (TryCollect(collector))
            {
                Collected?.Invoke(this);
            }
        }

        protected abstract bool TryCollect(PickupCollector collector);
    }
}
