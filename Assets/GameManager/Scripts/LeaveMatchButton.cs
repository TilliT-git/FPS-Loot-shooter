using Mirror;
using System;
using UnityEngine;
using UnityEngine.UI;

public class LeaveMatchButton : MonoBehaviour
{
    private Button _button;

    public static Action onLeaveMatch;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(LeaveMatch);
    }

    private void LeaveMatch()
    {
        if (NetworkManager.singleton == null) return;

        if (NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
            onLeaveMatch?.Invoke();
        }
        else if (NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopClient();
            onLeaveMatch?.Invoke();
        }
        else if (NetworkServer.active)
        {
            NetworkManager.singleton.StopServer();
            onLeaveMatch?.Invoke();
        }
    }
}
