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

        private string _validatedNick, _validatedTotalEnemyCount;
        public void Show(Action onStart)
        {
            _validatedNick = PlayerPrefsService.PlayerProfile.Nickname;
            _validatedTotalEnemyCount = PlayerPrefsService.SelectedEnemyCount.ToString();
            nicknameInputField.text = _validatedNick;
            totalEnemyCountInputField.text = _validatedTotalEnemyCount;
            _onStart = onStart;
            gameObject.SetActive(true);
        }
        public void OnPlayButtonClicked()
        {
            PlayerPrefsService.PlayerProfile = new CharacterProfileData(_validatedNick, PlayerPrefsService.PlayerProfile.Country);
            var parsedEnemy = int.TryParse(_validatedTotalEnemyCount, out var enemyCount);
            PlayerPrefsService.SelectedEnemyCount = parsedEnemy ? enemyCount : 3;
            _onStart?.Invoke();
            Hide();
        }
        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void ValidateNickInput(string nickInput)
        {
            _validatedNick = nickInput;
        }
        public void ValidateTotalEnemyCountInput(string countInput)
        {
            bool isValid;
            if (!int.TryParse(countInput, out _))
            {
                countInput = _validatedTotalEnemyCount;
                isValid = false;
            }
            else
            {
                if (int.Parse(countInput) <= 0)
                {
                    countInput = _validatedTotalEnemyCount;
                    isValid = false;
                }
                else
                {
                    isValid = true;
                }
            }
            if (!isValid)
            {
                totalEnemyCountInputField.text = _validatedTotalEnemyCount;
            }
            else{
                _validatedTotalEnemyCount = countInput;
            }
            
        }

    }
}
