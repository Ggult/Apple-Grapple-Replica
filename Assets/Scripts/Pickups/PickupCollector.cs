using System;
using System.Collections.Generic;
using UnityEngine;
namespace AppleGrapple
{
    // Marks a character as able to collect pickups; knows nothing about specific pickup types.
    public class PickupCollector : MonoBehaviour
    {
        [SerializeField] private CircleCollider2D pickupCollider;
        [SerializeField] private float pickupRange;

        private Dictionary<PickupType, List<Action>> pickupActions = new Dictionary<PickupType, List<Action>>();
        private void Awake()
        {
            pickupCollider.isTrigger = true;
            pickupCollider.radius = pickupRange;
        }
        public void RegisterPickupAction(PickupType pickupType , Action pickup)
        {
            if (pickupActions.ContainsKey(pickupType))
            {
                pickupActions[pickupType].Add(pickup);
            }
            else
            {
                pickupActions[pickupType] = new List<Action> { pickup };
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var pickup = other.GetComponentInParent<Pickup>();
            if (pickup == null) return;
            var collected = pickup.TryCollect();
            if (collected)
            {
                pickup.ReleaseReservation(null);
                if (pickupActions.TryGetValue(pickup.PickupType, out var actions))
                {
                    foreach (var action in actions)
                    {
                        action?.Invoke();
                    }
                }
            }
        }
        
    }
}
