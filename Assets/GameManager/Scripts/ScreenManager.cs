using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    [SerializeField] private GameObject _endScreenMatch;

    void Start()
    {
        GameManager.onEndMatch += ShowEndMatchScreen;
        GameManager.onStartMatch += HideEndMatchScreen;
    }

    private void OnDisable()
    {
        GameManager.onEndMatch -= ShowEndMatchScreen;
        GameManager.onStartMatch -= HideEndMatchScreen;
    }

    private void ShowEndMatchScreen()
    {
        _endScreenMatch.SetActive(true);
    }

    private void HideEndMatchScreen()
    {
        _endScreenMatch.SetActive(false);
    }
}
