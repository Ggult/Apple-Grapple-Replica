using System.Collections;
using UnityEngine;
namespace AppleGrapple
{
    // Purely visual: reacts to Health.Damaged with a hit-flash only. No physics, no movement control.
    // Body SpriteRenderer must use a material with the "AppleGrapple/SpriteFlash" shader (_FlashAmount property).
    [RequireComponent(typeof(CharacterRoot))]
    public class CharacterView : MonoBehaviour
    {

        private static readonly int FlashAmountId = Shader.PropertyToID("_FlashAmount");
        [SerializeField] private SpriteRenderer[] hitFlashSprites;
        [SerializeField] private float hitFlashDuration = 0.15f;
        [SerializeField] private CharacterInfoView characterInfoViewPrefab;
        private CharacterRoot _character;
        private CharacterInfoView _characterInfoView;
        private MaterialPropertyBlock _propertyBlock;
        private Coroutine _flashRoutine;


        private void Awake()
        {
            _character = GetComponent<CharacterRoot>();
            _propertyBlock = new MaterialPropertyBlock();
            _character.Health.Damaged += HandleDamaged;
            _character.Health.HealthChanged += HandleHealthChanged;
            _character.Health.Died += HandleDied;

            if (characterInfoViewPrefab != null && CharacterInfoCanvas.Instance != null)
            {
                _characterInfoView = Instantiate(characterInfoViewPrefab, CharacterInfoCanvas.Instance.RectTransform);
                _characterInfoView.SetFollowTarget(transform);
                _characterInfoView.Setup(_character.Identity.Data);
            }
        }

        private void Start()
        {
            if (_characterInfoView != null)
            {
                _characterInfoView.UpdateHealth(_character.Health.CurrentHealth, _character.Health.MaxHealth);
            }
        }

        private void OnDestroy()
        {
            _character.Health.Damaged -= HandleDamaged;
            _character.Health.HealthChanged -= HandleHealthChanged;
            _character.Health.Died -= HandleDied;
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

        private void HandleDied()
        {
            RemoveCharacterInfo();
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

