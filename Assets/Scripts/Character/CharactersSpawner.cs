using System.Collections.Generic;
using UnityEngine;

namespace AppleGrapple
{
    public class CharactersSpawner : MonoBehaviour
    {
        [SerializeField] private MapConfig _mapConfig;
        [SerializeField] private CharacterIdentity _playerPrefab;
        [SerializeField] private EnemyStateMachine _enemyPrefab;
        [SerializeField, Min(0f)] private float _boundaryMargin = 5f;
        [SerializeField, Min(0f)] private float _minimumSpawnDistance = 4f;
        [SerializeField, Min(1)] private int _positionAttempts = 40;

        private readonly List<GameObject> _spawnedCharacters = new();
        private readonly List<CharacterDeathController> _spawnedDeathControllers = new();

        public Transform Player { get; private set; }
        public IReadOnlyList<CharacterDeathController> SpawnedCharacters => _spawnedDeathControllers;

        public void SpawnCharacters(int enemyCount)
        {
            ClearSpawnedCharacters();
            CharacterDataPool.Reset();

            if (_playerPrefab == null || _enemyPrefab == null)
            {
                Debug.LogError("CharactersSpawner requires both player and enemy prefabs.", this);
                return;
            }

            var occupiedPositions = new List<Vector2>();
            var playerPosition = GetSpawnPosition(occupiedPositions);
            var player = Instantiate(_playerPrefab, playerPosition, Quaternion.identity);
            _spawnedCharacters.Add(player.gameObject);
            _spawnedDeathControllers.Add(player.GetComponent<CharacterDeathController>());
            occupiedPositions.Add(playerPosition);
            Player = player.transform;

            for (var i = 0; i < enemyCount; i++)
            {
                var enemyPosition = GetSpawnPosition(occupiedPositions);
                var enemy = Instantiate(_enemyPrefab, enemyPosition, Quaternion.identity);
                _spawnedCharacters.Add(enemy.gameObject);
                _spawnedDeathControllers.Add(enemy.GetComponent<CharacterDeathController>());
                occupiedPositions.Add(enemyPosition);
            }
        }
        public Transform GetPlayerStartTransform()
        {
            return Player;
        }
        private Vector2 GetSpawnPosition(List<Vector2> occupiedPositions)
        {
            if (_mapConfig == null)
            {
                Debug.LogWarning("MapConfig is not assigned on CharactersSpawner. Using origin-based fallback bounds.", this);
                return GetPositionWithMinimumDistance(Vector2.zero, Vector2.one * 100f, occupiedPositions);
            }

            var boundaryOffset = _mapConfig.boundarySettings.offset;
            var halfWidth = Mathf.Max(0f,
                (_mapConfig.GridX * 0.5f + 0.5f) * _mapConfig.tileWorldSize
                - boundaryOffset.x
                - _boundaryMargin);
            var halfHeight = Mathf.Max(0f,
                (_mapConfig.GridY * 0.5f + 0.5f) * _mapConfig.tileWorldSize
                - boundaryOffset.y
                - _boundaryMargin);

            return GetPositionWithMinimumDistance(Vector2.zero, new Vector2(halfWidth, halfHeight), occupiedPositions);
        }

        private Vector2 GetPositionWithMinimumDistance(Vector2 center, Vector2 halfExtents, List<Vector2> occupiedPositions)
        {
            var minimumDistanceSqr = _minimumSpawnDistance * _minimumSpawnDistance;
            var fallback = center;

            for (var attempt = 0; attempt < _positionAttempts; attempt++)
            {
                var candidate = new Vector2(
                    Random.Range(center.x - halfExtents.x, center.x + halfExtents.x),
                    Random.Range(center.y - halfExtents.y, center.y + halfExtents.y));
                fallback = candidate;

                if (IsFarEnough(candidate, occupiedPositions, minimumDistanceSqr))
                    return candidate;
            }

            Debug.LogWarning("Could not find a spawn position at the configured minimum distance. Using the last available position.", this);
            return fallback;
        }

        private bool IsFarEnough(Vector2 candidate, List<Vector2> occupiedPositions, float minimumDistanceSqr)
        {
            foreach (var occupiedPosition in occupiedPositions)
            {
                if ((candidate - occupiedPosition).sqrMagnitude < minimumDistanceSqr)
                    return false;
            }

            return true;
        }

        private void ClearSpawnedCharacters()
        {
            foreach (var character in _spawnedCharacters)
            {
                if (character != null)
                    Destroy(character);
            }

            _spawnedCharacters.Clear();
            _spawnedDeathControllers.Clear();
            Player = null;
        }
    }
}
