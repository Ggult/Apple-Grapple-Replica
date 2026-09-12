using ScratchCardAsset;
using UnityEngine;

namespace AppleGrapple
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private MapConfig mapConfig;
        private MapGenerator _mapGenerator;
        [SerializeField] private PickupSpawner pickupSpawner;
        [SerializeField] private CharactersSpawner charactersSpawner;
        [SerializeField] private CameraFollowController cameraFollow;
        [SerializeField] private UIManager uiManager;
        [SerializeField] private bool startRoundOnAwake;
        

        private void Awake()
        {
            _mapGenerator = new MapGenerator(mapConfig);
            GenerateMap();
        }

        private void Start()
        {
            if (startRoundOnAwake)
            {
                StartRound();
            }
            else
                uiManager?.ShowStartPanel(StartRound);
        }

        private void GenerateMap()
        {
            _mapGenerator.Generate();
        }
        public void StartRound()
        {
            if (charactersSpawner == null)
            {
                Debug.LogError("CharactersSpawner is not assigned in the GameManager.", this);
                return;
            }

            charactersSpawner.SpawnCharacters(PlayerPrefsService.SelectedEnemyCount);
            var player = charactersSpawner.GetPlayerStartTransform();
            if (player == null)
                return;

            uiManager?.HideStartPanel();

            if (cameraFollow != null)
            {
                cameraFollow.SetTarget(player);
            }
            else
            {
                Debug.LogWarning("CameraFollowController is not assigned in the GameManager.", this);
            }

            StartSpawnPickups();
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
