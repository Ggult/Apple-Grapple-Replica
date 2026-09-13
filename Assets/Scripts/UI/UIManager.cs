using System;
using UnityEngine;

namespace AppleGrapple
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private StartPanel _startPanel;
        [SerializeField] private ResultPanel _resultPanel;

        private void Awake()
        {
            CloseAll();
        }
        private void CloseAll()
        {
            if (_startPanel != null)
                _startPanel.Hide();

            if (_resultPanel != null)
                _resultPanel.Hide();
        }
        public void ShowStartPanel(Action onStart)
        {
            if (_startPanel == null)
            {
                Debug.LogWarning("StartPanel is not assigned in the UIManager.", this);
                return;
            }
            _startPanel.Show(onStart);
        }

        public void ShowResultPanel(bool isWin, Action onRestart)
        {
            if (_resultPanel == null)
            {
                Debug.LogWarning("ResultPanel is not assigned in the UIManager.", this);
                return;
            }
            _resultPanel.Show(isWin, onRestart);
        }
    }
}
