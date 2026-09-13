using UnityEngine;

namespace AppleGrapple
{
    public class CameraShakeFeedback : MonoBehaviour
    {
        [SerializeField] private Transform _cameraTransform;
        [SerializeField] private float _enemyHitDuration = 0.08f;
        [SerializeField] private float _swordClashDuration = 0.14f;
        [SerializeField] private float _positionMultiplier = 0.02f;
        [SerializeField] private float _frequency = 35f;

        private Vector3 _baseLocalPosition;
        private float _remaining;
        private float _duration;
        private float _noiseTime;

        private void Awake()
        {
            if (_cameraTransform == null)
                _cameraTransform = transform;

            _baseLocalPosition = _cameraTransform.localPosition;
        }

        private void OnEnable()
        {
            HitFeedback.Hit += OnHit;
        }

        private void OnDisable()
        {
            HitFeedback.Hit -= OnHit;

            if (_cameraTransform != null)
                _cameraTransform.localPosition = _baseLocalPosition;
        }

        private void LateUpdate()
        {
            if (_cameraTransform == null || _remaining <= 0f)
                return;

            _remaining -= Time.unscaledDeltaTime;
            _noiseTime += Time.unscaledDeltaTime * _frequency;

            var normalizedTime = Mathf.Clamp01(_remaining / _duration);
            var offset = new Vector2(
                Mathf.PerlinNoise(_noiseTime, 0f) - 0.5f,
                Mathf.PerlinNoise(0f, _noiseTime) - 0.5f);

            _cameraTransform.localPosition = _baseLocalPosition
                + (Vector3)(offset * (normalizedTime * _positionMultiplier));

            if (_remaining <= 0f)
                _cameraTransform.localPosition = _baseLocalPosition;
        }

        private void OnHit(HitInfo hit)
        {
            if (!hit.InvolvesPlayer)
                return;

            var duration = hit.HitType == HitType.SwordClash
                ? _swordClashDuration
                : _enemyHitDuration;

            _duration = Mathf.Max(_duration, duration);
            _remaining = Mathf.Max(_remaining, duration);
        }
    }
}
