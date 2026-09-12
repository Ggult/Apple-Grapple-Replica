using System.Collections;
using UnityEngine;
namespace AppleGrapple
{
    // Purely visual: reacts to Health.Damaged with a hit-flash only. No physics, no movement control.
    // Body SpriteRenderer must use a material with the "AppleGrapple/SpriteFlash" shader (_FlashAmount property).
    [RequireComponent(typeof(Health), typeof(CharacterIdentity))]
    public class CharacterView : MonoBehaviour
    {

        private static readonly int FlashAmountId = Shader.PropertyToID("_FlashAmount");
        [SerializeField] private SpriteRenderer[] hitFlashSprites;
        [SerializeField] private float hitFlashDuration = 0.15f;
        [SerializeField] private CharacterInfoView characterInfoViewPrefab;
        private Health _health;
        private CharacterIdentity _identity;
        private CharacterInfoView _characterInfoView;
        private MaterialPropertyBlock _propertyBlock;
        private Coroutine _flashRoutine;


        private void Awake()
        {
            _health = GetComponent<Health>();
            _identity = GetComponent<CharacterIdentity>();
            _propertyBlock = new MaterialPropertyBlock();
            _health.Damaged += HandleDamaged;
            _health.HealthChanged += HandleHealthChanged;

            if (characterInfoViewPrefab != null && CharacterInfoCanvas.Instance != null)
            {
                _characterInfoView = Instantiate(characterInfoViewPrefab, CharacterInfoCanvas.Instance.RectTransform);
                _characterInfoView.SetFollowTarget(transform);
                _characterInfoView.Setup(_identity.Data);
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

        private void HandleDamaged(HitInfo hitInfo)
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

        public void RemoveCharacterInfo()
        {
            if (_characterInfoView == null)
                return;

            Destroy(_characterInfoView.gameObject);
            _characterInfoView = null;
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

