using System.Collections.Generic;
using UnityEngine;

namespace AppleGrapple
{
    public class HitEffectPool : MonoBehaviour
    {
        [System.Serializable]
        private class HitEffectDefinition
        {
            public PooledHitEffect prefab;
            public Vector3 positionOffset;
            public float randomRotationZMin;
            public float randomRotationZMax;
        }

        [SerializeField] private List<HitEffectDefinition> _swordClashEffects = new();
        [SerializeField] private List<HitEffectDefinition> _enemyHitEffects = new();
        [SerializeField, HideInInspector] private PooledHitEffect _swordClashPrefab;
        [SerializeField, HideInInspector] private PooledHitEffect _secondarySwordClashPrefab;
        [SerializeField, HideInInspector] private Vector3 _secondarySwordClashRotation;
        [SerializeField, HideInInspector] private PooledHitEffect _enemyHitPrefab;
        [SerializeField] private int _initialPoolSize = 8;

        private static HitEffectPool _instance;
        private readonly Dictionary<int, Queue<PooledHitEffect>> _available = new();
        private const int EnemyHitPoolKey = -1;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            MigrateLegacySwordClashEffects();
            for (var i = 0; i < _swordClashEffects.Count; i++)
            {
                CreatePool(i, HitType.SwordClash, _swordClashEffects[i].prefab);
            }

            MigrateLegacyEnemyHitEffect();
            for (var i = 0; i < _enemyHitEffects.Count; i++)
            {
                CreatePool(GetPoolKey(HitType.EnemyHit, i), HitType.EnemyHit, _enemyHitEffects[i].prefab);
            }
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

        private void MigrateLegacySwordClashEffects()
        {
            if (_swordClashPrefab != null)
            {
                _swordClashEffects.Insert(0, new HitEffectDefinition
                {
                    prefab = _swordClashPrefab
                });
            }

            if (_secondarySwordClashPrefab != null)
            {
                _swordClashEffects.Insert(Mathf.Min(1, _swordClashEffects.Count), new HitEffectDefinition
                {
                    prefab = _secondarySwordClashPrefab,
                    randomRotationZMin = _secondarySwordClashRotation.z,
                    randomRotationZMax = _secondarySwordClashRotation.z
                });
            }
        }

        private void MigrateLegacyEnemyHitEffect()
        {
            if (_enemyHitPrefab != null && _enemyHitEffects.Count == 0)
            {
                _enemyHitEffects.Add(new HitEffectDefinition
                {
                    prefab = _enemyHitPrefab
                });
            }
        }

        private int GetPoolKey(HitType type, int effectIndex)
        {
            return type == HitType.EnemyHit ? EnemyHitPoolKey - effectIndex : effectIndex;
        }

        private void CreatePool(int poolKey, HitType type, PooledHitEffect prefab)
        {
            if (prefab == null)
                return;

            var pool = new Queue<PooledHitEffect>();
            _available[poolKey] = pool;

            for (var i = 0; i < _initialPoolSize; i++)
            {
                pool.Enqueue(Create(type, prefab, poolKey));
            }
        }

        private PooledHitEffect Create(HitType type, PooledHitEffect prefab, int poolKey)
        {
            var effect = Instantiate(prefab, transform);
            effect.Initialize(this, type, poolKey);
            effect.gameObject.SetActive(false);
            return effect;
        }

        private void PlayInternal(HitType type, Vector3 position)
        {
            if (type == HitType.SwordClash)
            {
                for (var i = 0; i < _swordClashEffects.Count; i++)
                {
                    PlayEffect(i, type, position, _swordClashEffects[i]);
                }
                return;
            }

            for (var i = 0; i < _enemyHitEffects.Count; i++)
            {
                PlayEffect(GetPoolKey(type, i), type, position, _enemyHitEffects[i]);
            }
        }

        private void PlayEffect(int poolKey, HitType type, Vector3 position, HitEffectDefinition definition)
        {
            if (definition.prefab == null || !_available.TryGetValue(poolKey, out var pool))
                return;

            var effect = pool.Count > 0
                ? pool.Dequeue()
                : Create(type, definition.prefab, poolKey);
            effect.transform.SetPositionAndRotation(
                position + definition.positionOffset,
                Quaternion.Euler(0f, 0f, Random.Range(
                    definition.randomRotationZMin,
                    definition.randomRotationZMax)));
            effect.Play();
        }

        public void Return(PooledHitEffect effect, HitType type, int poolKey)
        {
            effect.StopAndClear();
            effect.transform.SetParent(transform);
            effect.gameObject.SetActive(false);

            if (!_available.TryGetValue(poolKey, out var pool))
            {
                pool = new Queue<PooledHitEffect>();
                _available[poolKey] = pool;
            }

            pool.Enqueue(effect);
        }
    }
}
