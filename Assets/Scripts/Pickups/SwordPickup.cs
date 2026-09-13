namespace AppleGrapple
{
    public class SwordPickup : Pickup
    {
        public override bool TryCollect()
        {
            Collected?.Invoke(this);
            return true;
        }
    }
}
