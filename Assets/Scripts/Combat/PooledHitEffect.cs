using UnityEngine;

namespace AppleGrapple
{
    public class PooledHitEffect : MonoBehaviour
    {
        [SerializeField] private ParticleSystem[] _particleSystems;
        private HitEffectPool _pool;
        private HitType _type;
        private int _poolKey;
        private bool _isReturned;

        private void Awake()
        {
            if (_particleSystems == null || _particleSystems.Length == 0)
                return;

            foreach (var particleSystem in _particleSystems)
            {
                var main = particleSystem.main;
                main.stopAction = ParticleSystemStopAction.Callback;
            }
        }

        public void Initialize(HitEffectPool pool, HitType type)
        {
            Initialize(pool, type, 0);
        }

        public void Initialize(HitEffectPool pool, HitType type, int poolKey)
        {
            _pool = pool;
            _type = type;
            _poolKey = poolKey;
        }

        public void Play()
        {
            _isReturned = false;
            gameObject.SetActive(true);
            foreach (var particleSystem in _particleSystems)
            {
                particleSystem.Play(true);
            }
        }

        public void StopAndClear()
        {
            foreach (var particleSystem in _particleSystems)
            {
                particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }

        private void OnParticleSystemStopped()
        {
            if (!_isReturned && _pool != null)
            {
                _isReturned = true;
                _pool.Return(this, _type, _poolKey);
            }
        }
    }
}
