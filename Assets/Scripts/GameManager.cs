using System.Collections.Generic;
using ScratchCardAsset;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        private readonly List<CharacterDeathController> _roundCharacters = new();
        private bool _roundActive;
        

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
        private void Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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

            UnsubscribeFromDeaths();
            charactersSpawner.SpawnCharacters(PlayerPrefsService.SelectedEnemyCount);
            SubscribeToDeaths();
            var player = charactersSpawner.GetPlayerStartTransform();
            if (player == null)
                return;

            if (cameraFollow != null)
            {
                cameraFollow.SetTarget(player);
            }
            else
            {
                Debug.LogWarning("CameraFollowController is not assigned in the GameManager.", this);
            }

            StartSpawnPickups();
            _roundActive = true;

            if (GetAliveEnemyCount() == 0)
            {
                EndRound(true);
            }
        }

        private void SubscribeToDeaths()
        {
            _roundCharacters.Clear();
            foreach (var character in charactersSpawner.SpawnedCharacters)
            {
                if (character == null)
                    continue;

                character.Died += HandleCharacterDied;
                _roundCharacters.Add(character);
            }
        }

        private void UnsubscribeFromDeaths()
        {
            foreach (var character in _roundCharacters)
            {
                if (character != null)
                    character.Died -= HandleCharacterDied;
            }

            _roundCharacters.Clear();
        }

        private void HandleCharacterDied(CharacterDeathController character)
        {
            if (!_roundActive)
                return;

            if (character.IsPlayer)
            {
                EndRound(false);
                return;
            }

            if (GetAliveEnemyCount() == 0)
            {
                EndRound(true);
            }
        }

        private int GetAliveEnemyCount()
        {
            var aliveEnemies = 0;
            foreach (var character in _roundCharacters)
            {
                if (character != null && !character.IsPlayer && !character.IsDead)
                    aliveEnemies++;
            }

            return aliveEnemies;
        }

        private void EndRound(bool playerWon)
        {
            if (!_roundActive)
                return;

            _roundActive = false;
            pickupSpawner?.StopSpawning();
            StopRoundCharacterMovement();
            UnsubscribeFromDeaths();

            uiManager?.ShowResultPanel(playerWon, Restart);
        }

        private void StopRoundCharacterMovement()
        {
            foreach (var character in _roundCharacters)
            {
                character?.StopMovement();
            }
        }

        private void OnDestroy()
        {
            UnsubscribeFromDeaths();
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
