using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AppleGrapple
{
    public sealed class GameBootstrapper : MonoBehaviour
    {
        [SerializeField] private MapController mapController;
        [SerializeField] private PickupSpawner pickupSpawner;
        [SerializeField] private CharactersSpawner charactersSpawner;
        [SerializeField] private CameraFollowController cameraFollow;
        [SerializeField] private UIManager uiManager;
        [SerializeField] private bool startRoundOnAwake;

        private readonly List<CharacterRoot> _roundCharacters = new();
        private bool _roundActive;

        private void Awake()
        {
            if (mapController == null)
            {
                Debug.LogError("MapController is not assigned on the GameBootstrapper.", this);
                return;
            }

            mapController.Generate();
        }

        private void Start()
        {
            if (startRoundOnAwake)
                StartRound();
            else
                uiManager?.ShowStartPanel(StartRound);
        }

        public void StartRound()
        {
            if (charactersSpawner == null)
            {
                Debug.LogError("CharactersSpawner is not assigned on the GameBootstrapper.", this);
                return;
            }

            UnsubscribeFromDeaths();
            charactersSpawner.SpawnCharacters(PlayerPrefsService.SelectedEnemyCount);
            SubscribeToDeaths();
            var player = charactersSpawner.GetPlayerStartTransform();
            if (player == null)
                return;

            cameraFollow?.SetTarget(player);
            StartSpawnPickups();
            _roundActive = true;

            if (GetAliveEnemyCount() == 0)
                EndRound(true);
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

        private void HandleCharacterDied(CharacterRoot character)
        {
            if (!_roundActive)
                return;

            if (character.IsPlayer)
            {
                EndRound(false);
                return;
            }

            if (GetAliveEnemyCount() == 0)
                EndRound(true);
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
                character?.StopMovement();
        }

        private void Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void OnDestroy()
        {
            UnsubscribeFromDeaths();
        }

        private void StartSpawnPickups()
        {
            if (pickupSpawner != null)
                pickupSpawner.StartSpawning();
            else
                Debug.LogWarning("PickupSpawner is not assigned on the GameBootstrapper.", this);
        }
    }
}