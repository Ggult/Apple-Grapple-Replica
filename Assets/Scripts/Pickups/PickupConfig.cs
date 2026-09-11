using UnityEngine;
namespace AppleGrapple
{
    [CreateAssetMenu(fileName = "PickupConfig", menuName = "AppleGrapple/Pickup Config")]
    public class PickupConfig : ScriptableObject
    {
        public Pickup prefab;
        public int poolSize = 10;
        public int maxAliveCount = 5;
        public float minSpawnInterval = 3f;
        public float maxSpawnInterval = 8f;
    }
}
