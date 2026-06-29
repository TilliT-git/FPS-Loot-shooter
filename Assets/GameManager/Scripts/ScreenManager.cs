using System;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    [SerializeField] private GameObject _endScreenMatch;
    [SerializeField] private GameObject _pauseMenuScreen;
    [SerializeField] private GameObject _playerCanvasUI;

    private bool _isPaused;

    public static Action <bool> onPaused;

    void Awake()
    {
        LeaveMatchButton.onLeaveMatch += HideAllPlayerScreens;
        GameManager.onEndMatch += ShowEndMatchScreen;
        GameManager.onStartMatch += HideEndMatchScreen;
    }

    private void OnDisable()
    {
        LeaveMatchButton.onLeaveMatch -= HideAllPlayerScreens;
        GameManager.onEndMatch -= ShowEndMatchScreen;
        GameManager.onStartMatch -= HideEndMatchScreen;
    }

    private void Update()
    {
        if (_pauseMenuScreen == null) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _isPaused = !_isPaused;

            _pauseMenuScreen.SetActive(_isPaused);

            Cursor.visible = _isPaused;

            if (_isPaused)
            {
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
            }

            onPaused?.Invoke(_isPaused);
        }
    }

    private void ShowEndMatchScreen()
    {
        _endScreenMatch.SetActive(true);
    }

    private void HideEndMatchScreen()
    {
        _endScreenMatch.SetActive(false);
    }

    private void HideAllPlayerScreens()
    {
        _isPaused = false;
        _pauseMenuScreen.SetActive(false);
        _playerCanvasUI.SetActive(false);
    }
}
