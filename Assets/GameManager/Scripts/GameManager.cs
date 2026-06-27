using Mirror;
using System;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    [SerializeField] private int _killsToWin;
    public int KillsToWin => _killsToWin;
    private GameTimer _gameTimer;

    public static Action onEndMatch;

    public override void OnStartServer()
    {
        base.OnStartServer();

        _gameTimer = GetComponent<GameTimer>();

        GameTimer.onTimeOut += EndMatch;
        PlayerStats.onPlayerMadeKill += CheckFowWin;
    }

    private void OnDisable()
    {
        GameTimer.onTimeOut -= EndMatch;
        PlayerStats.onPlayerMadeKill -= CheckFowWin;
    }

    [Server]
    private void CheckFowWin(PlayerStats playerStats)
    {
        if (playerStats.Kills >= _killsToWin)
        {
            EndMatch();
        }
    }

    private void EndMatch()
    {
        Debug.Log("TIMEOUT");
        onEndMatch?.Invoke();
    }
}
