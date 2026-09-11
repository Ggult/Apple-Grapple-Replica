using System.Collections;
using UnityEngine;
using UnityEngine.UI;
namespace AppleGrapple
{
    // Purely visual, like CharacterView: reflects Health's current/max ratio via Image.fillAmount.
    // Has no Health reference of its own — CharacterView tells it what to show.
    // _whiteBar/_activeBar must be Image Type = Filled (e.g. Fill Method = Horizontal).
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private Image _whiteBar, _activeBar;
        [SerializeField] private float _whiteBarDelay = 0.5f;
        [SerializeField] private float _whiteBarLerpDuration = 0.3f;

        private Coroutine _whiteBarRoutine;

        public void UpdateFill(int current, int max)
        {
            var ratio = max > 0 ? (float)current / max : 0f;

            _activeBar.fillAmount = ratio;

            if (_whiteBarRoutine != null)
            {
                StopCoroutine(_whiteBarRoutine);
            }
            _whiteBarRoutine = StartCoroutine(DelayedWhiteBarRoutine(ratio));
        }

        private IEnumerator DelayedWhiteBarRoutine(float targetFill)
        {
            yield return new WaitForSeconds(_whiteBarDelay);

            var startFill = _whiteBar.fillAmount;
            var elapsed = 0f;
            while (elapsed < _whiteBarLerpDuration)
            {
                _whiteBar.fillAmount = Mathf.Lerp(startFill, targetFill, elapsed / _whiteBarLerpDuration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            _whiteBar.fillAmount = targetFill;
            _whiteBarRoutine = null;
        }
    }
}
