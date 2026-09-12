using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AppleGrapple
{
    // One spawner instance = one pickup type, with its own pool/interval/limit (per-type settings via PickupConfig).
    public class PickupSpawner : MonoBehaviour
    {
        [SerializeField] private PickupConfig _config;
        [SerializeField] private MapConfig _mapConfig;
        [SerializeField] private float _arenaMargin = 1f;

        private readonly Queue<Pickup> _pool = new();
        private readonly List<Pickup> _alive = new();
        private Coroutine _spawnRoutine;

        private void Awake()
        {
            for (int i = 0; i < _config.poolSize; i++)
            {
                CreatePooled();
            }
        }

        public void StartSpawning()
        {
            if (_spawnRoutine == null)
            {
                SpawnInitialPickups();
                _spawnRoutine = StartCoroutine(SpawnLoop());
            }
        }

        public void StopSpawning()
        {
            if (_spawnRoutine != null)
            {
                StopCoroutine(_spawnRoutine);
                _spawnRoutine = null;
            }
        }

        private IEnumerator SpawnLoop()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(_config.minSpawnInterval, _config.maxSpawnInterval));

                if (_alive.Count < _config.maxAliveCount)
                {
                    Spawn();
                }
            }
        }

        private void SpawnInitialPickups()
        {
            var initialCount = Mathf.Min(_config.initialSpawnCount, _config.maxAliveCount);
            for (var i = 0; i < initialCount; i++)
            {
                Spawn();
            }
        }

        private Pickup CreatePooled()
        {
            var pickup = Instantiate(_config.prefab, transform);
            pickup.gameObject.SetActive(false);
            _pool.Enqueue(pickup);
            return pickup;
        }

        private void Spawn()
        {
            if (_pool.Count == 0)
            {
                CreatePooled();
            }

            var pickup = _pool.Dequeue();
            pickup.transform.SetParent(null);
            pickup.transform.position = GetRandomArenaPosition();
            pickup.gameObject.SetActive(true);
            pickup.Collected += OnPickupCollected;
            _alive.Add(pickup);
        }

        private void OnPickupCollected(Pickup pickup)
        {
            pickup.Collected -= OnPickupCollected;
            _alive.Remove(pickup);
            pickup.gameObject.SetActive(false);
            pickup.transform.SetParent(transform);
            _pool.Enqueue(pickup);
        }

        private Vector3 GetRandomArenaPosition()
        {
            var halfWidth = _mapConfig.GridX * 0.5f * _mapConfig.tileWorldSize - _arenaMargin;
            var halfHeight = _mapConfig.GridY * 0.5f * _mapConfig.tileWorldSize - _arenaMargin;
            var x = Random.Range(-halfWidth, halfWidth);
            var y = Random.Range(-halfHeight, halfHeight);
            return new Vector3(x, y, 0f);
        }
    }
}

