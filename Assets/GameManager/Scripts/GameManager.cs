using Mirror;
using System;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    [SerializeField] private int _killsToWin;
    public int KillsToWin => _killsToWin;
    private GameTimer _gameTimer;

    public static Action onStartMatch;
    public static Action onEndMatch;

    public override void OnStartServer()
    {
        base.OnStartServer();

        _gameTimer = GetComponent<GameTimer>();

        GameTimer.onTimeOut += EndMatch;
        PlayerStats.onPlayerMadeKill += CheckFowWin;
        StartMatchButton.onStartButtonClicked += StartMatch;
    }

    private void OnDisable()
    {
        GameTimer.onTimeOut -= EndMatch;
        PlayerStats.onPlayerMadeKill -= CheckFowWin;
        StartMatchButton.onStartButtonClicked -= StartMatch;
    }

    [Server]
    private void CheckFowWin(PlayerStats playerStats)
    {
        if (playerStats.Kills >= _killsToWin)
        {
            EndMatch();
        }
    }

    [Server]
    private void StartMatch()
    {
        Debug.Log("StartMatch");
        onStartMatch?.Invoke();
        RpcNotifyClientsMatchStarted();
    }

    [Server]
    private void EndMatch()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Debug.Log("TIMEOUT");
        onEndMatch?.Invoke();
        RpcNotifyClientsMatchEnded();
    }

    [ClientRpc]
    private void RpcNotifyClientsMatchStarted()
    {
        onStartMatch?.Invoke();
    }

    [ClientRpc]
    private void RpcNotifyClientsMatchEnded()
    {
        onEndMatch?.Invoke();
    }
}
