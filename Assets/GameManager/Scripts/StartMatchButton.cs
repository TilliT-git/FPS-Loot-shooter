using System;
using UnityEngine;
using UnityEngine.UI;

public class StartMatchButton : MonoBehaviour
{
    private Button _button;

    public static Action onStartButtonClicked;

    private void Awake()
    {
        _button = GetComponent<Button>();

        _button.onClick.AddListener(HandleClick);
    }

    private void HandleClick()
    {
        onStartButtonClicked?.Invoke();
    }
}
