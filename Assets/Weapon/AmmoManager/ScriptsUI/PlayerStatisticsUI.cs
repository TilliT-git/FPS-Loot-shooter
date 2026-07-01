using TMPro;
using UnityEngine;

public class PlayerStatisticsUI : MonoBehaviour
{
    private PlayerStats _playerStats;
    [SerializeField] private TextMeshProUGUI _statisticsText;

    private void Start()
    {
        if (_playerStats != null)
        {
            _playerStats.onPlayerStatsChange += UpdateStatistics;

            UpdateStatistics(_playerStats.Kills, _playerStats.Deaths);
        }
    }

    public void Initialize(PlayerStats playerStats)
    {
        _playerStats = playerStats;
    }

    private void OnDisable()
    {
        if (_playerStats != null)
        {
            _playerStats.onPlayerStatsChange -= UpdateStatistics;
        }
    }

    private void UpdateStatistics(int kills, int deaths)
    {
        if (_statisticsText == null) return;

        _statisticsText.text = $"{kills} K / {deaths} D";
    }
}
