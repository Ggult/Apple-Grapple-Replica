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
        private EnemyStateMachine _reservedBy;

        private void OnEnable()
        {
            PickupRegistry.Instance?.Register(this);
        }

        private void OnDisable()
        {
            PickupRegistry.Instance?.Unregister(this);
        }
       
        public abstract bool TryCollect();

        public bool IsReservedByOther(EnemyStateMachine requester)
        {
            return _reservedBy != null && _reservedBy != requester;
        }

        public bool TryReserve(EnemyStateMachine requester)
        {
            if (requester == null || IsReservedByOther(requester))
                return false;

            _reservedBy = requester;
            return true;
        }

        public void ReleaseReservation(EnemyStateMachine requester)
        {
            if (requester == null || _reservedBy == requester)
                _reservedBy = null;
        }
    }
}
