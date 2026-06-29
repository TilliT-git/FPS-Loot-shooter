using Mirror;
using System;
using UnityEngine.UI;

public class StartMatchButton : NetworkBehaviour
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
