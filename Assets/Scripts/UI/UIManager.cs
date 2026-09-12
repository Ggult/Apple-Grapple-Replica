using System;
using UnityEngine;

namespace AppleGrapple
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private StartPanel _startPanel;

        public void ShowStartPanel(Action onStart)
        {
            if (_startPanel == null)
            {
                Debug.LogWarning("StartPanel is not assigned in the UIManager.", this);
                return;
            }
            _startPanel.Show(onStart);
        }

        public void HideStartPanel()
        {
            if (_startPanel == null)
                return;

            _startPanel.Hide();
        }
    }
}
