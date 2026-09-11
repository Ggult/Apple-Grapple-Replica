using UnityEngine;

namespace AppleGrapple
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private MapConfig mapConfig;
        private MapGenerator _mapGenerator;
        [SerializeField] private PickupSpawner pickupSpawner;
        private void Awake()
        {
            _mapGenerator = new MapGenerator(mapConfig);

            GenerateMap();
            StartSpawnPickups();
        }
        private void GenerateMap()
        {
            _mapGenerator.Generate();
        }
        private void StartSpawnPickups()
        {
            if (pickupSpawner != null)
            {
                pickupSpawner.StartSpawning();
            }
            else
                Debug.LogWarning("PickupSpawner is not assigned in the GameManager !");
        }
    }
}
