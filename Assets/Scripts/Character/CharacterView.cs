using System.Collections;
using UnityEngine;
namespace AppleGrapple
{
    // Purely visual: reacts to Health.Damaged with a hit-flash only. No physics, no movement control.
    // Body SpriteRenderer must use a material with the "AppleGrapple/SpriteFlash" shader (_FlashAmount property).
    [RequireComponent(typeof(Health))]
    public class CharacterView : MonoBehaviour
    {

        private static readonly int FlashAmountId = Shader.PropertyToID("_FlashAmount");
        [SerializeField] private SpriteRenderer[] hitFlashSprites;
        [SerializeField] private float hitFlashDuration = 0.15f;
        [SerializeField] private CharacterInfoView characterInfoViewPrefab;
        [SerializeField] private bool isPlayer;
        private Health _health;
        private CharacterInfoView _characterInfoView;
        private MaterialPropertyBlock _propertyBlock;
        private Coroutine _flashRoutine;


        private void Awake()
        {
            _health = GetComponent<Health>();
            _propertyBlock = new MaterialPropertyBlock();
            _health.Damaged += HandleDamaged;
            _health.HealthChanged += HandleHealthChanged;

            if (characterInfoViewPrefab != null && CharacterInfoCanvas.Instance != null)
            {
                var data = isPlayer ? PlayerIdentity.Load() : CharacterDataPool.Get();
                _characterInfoView = Instantiate(characterInfoViewPrefab, CharacterInfoCanvas.Instance.RectTransform);
                _characterInfoView.SetFollowTarget(transform);
                _characterInfoView.Setup(data);
            }
        }

        private void Start()
        {
            if (_characterInfoView != null)
            {
                _characterInfoView.UpdateHealth(_health.CurrentHealth, _health.MaxHealth);
            }
        }

        private void OnDestroy()
        {
            _health.Damaged -= HandleDamaged;
            _health.HealthChanged -= HandleHealthChanged;
        }

        private void HandleDamaged(Vector2 sourcePosition)
        {
            FlashWhite();
        }

        private void HandleHealthChanged(int current, int max)
        {
            if (_characterInfoView != null)
            {
                _characterInfoView.UpdateHealth(current, max);
            }
        }

        private void FlashWhite()
        {
            if (_flashRoutine != null)
            {
                StopCoroutine(_flashRoutine);
            }
            _flashRoutine = StartCoroutine(FlashRoutine());
        }

        private IEnumerator FlashRoutine()
        {
            SetFlashAmount(1f);
            yield return new WaitForSeconds(hitFlashDuration);
            SetFlashAmount(0f);
            _flashRoutine = null;
        }

        private void SetFlashAmount(float amount)
        {
            foreach (var sprite in hitFlashSprites)
            {
                sprite.GetPropertyBlock(_propertyBlock);
                _propertyBlock.SetFloat(FlashAmountId, amount);
                sprite.SetPropertyBlock(_propertyBlock);
            }
        }
    }
}

