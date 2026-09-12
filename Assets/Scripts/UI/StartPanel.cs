using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AppleGrapple
{
    public class StartPanel : MonoBehaviour
    {
        [SerializeField] private TMP_InputField nicknameInputField;
        [SerializeField] private TMP_InputField totalEnemyCountInputField;
        
        private Action _onStart;
        public void Show(Action onStart)
        {
            nicknameInputField.text = PlayerPrefsService.PlayerProfile.Nickname;
            totalEnemyCountInputField.text = PlayerPrefsService.SelectedEnemyCount.ToString();
            _onStart = onStart;
            gameObject.SetActive(true);
        }
        public void OnPlayButtonClicked()
        {
            PlayerPrefsService.PlayerProfile = new CharacterProfileData(nicknameInputField.text, PlayerPrefsService.PlayerProfile.Country);
            PlayerPrefsService.SelectedEnemyCount = int.Parse(totalEnemyCountInputField.text);
            _onStart?.Invoke();
        }
        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
