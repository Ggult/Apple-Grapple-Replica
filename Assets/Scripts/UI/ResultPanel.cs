using UnityEngine;
using System;
using TMPro;

namespace AppleGrapple
{
    public class ResultPanel : MonoBehaviour
    {
        private Action _onRestart;

        [SerializeField] private TextMeshProUGUI resultText;

        public void Show(bool isWin, Action onRestart)
        {
            gameObject.SetActive(true);
            _onRestart = onRestart;
            resultText.text = isWin ? "You Win!" : "Unlucky!";
        }
        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void OnRestartButtonClicked()
        {
            _onRestart?.Invoke();
            Hide();
        }
    }
}
