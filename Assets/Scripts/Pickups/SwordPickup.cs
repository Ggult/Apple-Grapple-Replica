using UnityEngine;
namespace AppleGrapple
{
    public class SwordPickup : Pickup
    {
        protected override bool TryCollect(PickupCollector collector)
        {
            var swordOrigin = collector.GetCapability<SwordOrigin>();
            if (swordOrigin == null) return false;

            swordOrigin.AddWeapon();
            return true;
        }
    }
}
