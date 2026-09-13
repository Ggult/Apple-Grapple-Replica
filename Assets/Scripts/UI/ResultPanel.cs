using UnityEngine;
using System;
public class ResultPanel : MonoBehaviour
{
    private Action _onRestart;
    public void Show(bool isWin, Action onRestart)
    {
        gameObject.SetActive(true);
        _onRestart = onRestart;
        // You can add additional logic here to display win/lose state based on isWin
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
