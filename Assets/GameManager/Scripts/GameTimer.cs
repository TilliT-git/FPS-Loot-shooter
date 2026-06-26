using Mirror;
using TMPro;
using UnityEngine;

public class GameTimer : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnTimeRemainingChanged))]
    [SerializeField] private float _timeRemaining;
    [SerializeField] private TextMeshProUGUI _timerText;

    private bool _isTimerRunning = false;

    public override void OnStartServer()
    {
        base.OnStartServer();
        _isTimerRunning = true;
    }

    private void Update()
    {
        if (!isServer || !_isTimerRunning) return;

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

    private void OnTimerEnd()
    {
        Debug.Log("TIME OUT");
    }
}
