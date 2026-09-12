using System.Collections.Generic;
using UnityEngine;

namespace AppleGrapple
{
    public class HitEffectPool : MonoBehaviour
    {
        [SerializeField] private PooledHitEffect _swordClashPrefab;
        [SerializeField] private PooledHitEffect _enemyHitPrefab;
        [SerializeField] private int _initialPoolSize = 8;

        private static HitEffectPool _instance;
        private readonly Dictionary<HitType, Queue<PooledHitEffect>> _available = new();

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            CreatePool(HitType.SwordClash, _swordClashPrefab);
            CreatePool(HitType.EnemyHit, _enemyHitPrefab);
            HitFeedback.Hit += OnHit;
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                HitFeedback.Hit -= OnHit;
                _instance = null;
            }
        }

        private void OnHit(HitInfo hit)
        {
            PlayInternal(hit.HitType, hit.Position);
        }

        private void CreatePool(HitType type, PooledHitEffect prefab)
        {
            if (prefab == null)
                return;

            var pool = new Queue<PooledHitEffect>();
            _available[type] = pool;

            for (var i = 0; i < _initialPoolSize; i++)
            {
                pool.Enqueue(Create(type, prefab));
            }
        }

        private PooledHitEffect Create(HitType type, PooledHitEffect prefab)
        {
            var effect = Instantiate(prefab, transform);
            effect.Initialize(this, type);
            effect.gameObject.SetActive(false);
            return effect;
        }

        private void PlayInternal(HitType type, Vector3 position)
        {
            if (!_available.TryGetValue(type, out var pool))
                return;

            var prefab = GetPrefab(type);
            if (prefab == null)
                return;

            var effect = pool.Count > 0 ? pool.Dequeue() : Create(type, prefab);
            effect.transform.SetPositionAndRotation(position, Quaternion.identity);
            effect.Play();
        }

        private PooledHitEffect GetPrefab(HitType type)
        {
            return type == HitType.SwordClash ? _swordClashPrefab : _enemyHitPrefab;
        }

        public void Return(PooledHitEffect effect, HitType type)
        {
            effect.StopAndClear();
            effect.transform.SetParent(transform);
            effect.gameObject.SetActive(false);

            if (!_available.TryGetValue(type, out var pool))
            {
                pool = new Queue<PooledHitEffect>();
                _available[type] = pool;
            }

            pool.Enqueue(effect);
        }
    }
}
