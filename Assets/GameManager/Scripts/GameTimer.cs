using Mirror;
using System;
using TMPro;
using UnityEngine;

public class GameTimer : NetworkBehaviour
{
    [SerializeField] private float _startTimeRemaining;
    [SerializeField] private TextMeshProUGUI _timerText;

    [SyncVar(hook = nameof(OnTimeRemainingChanged))]
    private float _timeRemaining;

    private bool _isTimerRunning = false;
    public static Action onTimeOut;

    public override void OnStartServer()
    {
        base.OnStartServer();
        _isTimerRunning = true;
    }

    private void Awake()
    {
        _timeRemaining = _startTimeRemaining;

        GameManager.onEndMatch += StopTimer;
        GameManager.onStartMatch += SetTimer;
    }

    private void OnDestroy()
    {
        GameManager.onEndMatch -= StopTimer;
        GameManager.onStartMatch -= SetTimer;
    }

    private void Update()
    {
        if (!isServer) return;

        if (_isTimerRunning)
        {
            if (_timeRemaining > 0)
            {
                _timeRemaining -= Time.deltaTime;
                DisplayTime(_timeRemaining);
            }
            else
            {
                _timeRemaining = 0;
                _isTimerRunning = false;
                OnTimerEnd();
            }
        }
    }

    private void OnTimeRemainingChanged(float oldTime, float newTime)
    {
        DisplayTime(newTime);
    }

    private void DisplayTime(float timeToDisplay)
    {
        if (timeToDisplay < 0) timeToDisplay = 0;

        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        _timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void SetTimer()
    {
        if (!isServer) return;

        _timeRemaining = _startTimeRemaining;
        _isTimerRunning = true;
    }

    private void StopTimer()
    {
        if (!isServer) return;

        _isTimerRunning = false;
    }

    private void OnTimerEnd()
    {
        onTimeOut?.Invoke();
    }
}
