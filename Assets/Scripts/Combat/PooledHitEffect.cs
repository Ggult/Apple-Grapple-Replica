using UnityEngine;

namespace AppleGrapple
{
    [RequireComponent(typeof(ParticleSystem))]
    public class PooledHitEffect : MonoBehaviour
    {
        private ParticleSystem _particleSystem;
        private HitEffectPool _pool;
        private HitType _type;
        private bool _isReturned;

        private void Awake()
        {
            _particleSystem = GetComponent<ParticleSystem>();
            var main = _particleSystem.main;
            main.stopAction = ParticleSystemStopAction.Callback;
        }

        public void Initialize(HitEffectPool pool, HitType type)
        {
            _pool = pool;
            _type = type;
        }

        public void Play()
        {
            _isReturned = false;
            gameObject.SetActive(true);
            _particleSystem.Play(true);
        }

        public void StopAndClear()
        {
            _particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        private void OnParticleSystemStopped()
        {
            if (!_isReturned && _pool != null)
            {
                _isReturned = true;
                _pool.Return(this, _type);
            }
        }
    }
}
