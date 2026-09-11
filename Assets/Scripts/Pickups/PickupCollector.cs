using UnityEngine;
namespace AppleGrapple
{
    // Marks a character as able to collect pickups; knows nothing about specific pickup types.
    public class PickupCollector : MonoBehaviour
    {
        // Each Pickup asks for whatever capability component it needs (SwordOrigin, Health, Inventory, ...).
        public T GetCapability<T>() where T : class => GetComponent<T>() ?? GetComponentInChildren<T>();
    }
}
