using System;
using UnityEngine;
namespace AppleGrapple
{
    [Serializable]
    public enum PickupType
    {
        None,
        Sword
    }
    // Base for ground pickups: spawner owns pooling, subclasses only decide what collecting does.
    [RequireComponent(typeof(Collider2D))]
    public abstract class Pickup : MonoBehaviour
    {
        public Action<Pickup> Collected;
        [SerializeField] private PickupType pickupType;
        public PickupType PickupType => pickupType;
       
        public abstract bool TryCollect();
    }
}
