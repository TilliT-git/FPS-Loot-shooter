using Mirror;
using UnityEngine;

public class PlayerUISpawner : NetworkBehaviour
{
    [SerializeField] private GameObject _uiCanvasPrefab;
    private GameObject _uiInstance;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        if (_uiInstance == null)
        {
            _uiInstance = Instantiate(_uiCanvasPrefab);
        }

        AmmoUI ammoUI = _uiInstance.GetComponentInChildren<AmmoUI>();
        WeaponChanger weaponChanger = GetComponentInChildren<WeaponChanger>();

        if (ammoUI != null && weaponChanger != null)
        {
            ammoUI.Initialize(weaponChanger);
        }

        PlayerHealthUI healthUI = _uiInstance.GetComponentInChildren<PlayerHealthUI>();
        PlayerHealth health = GetComponent<PlayerHealth>();

        if (healthUI != null && health != null)
        {
            healthUI.Initialize(health);
        }

        PlayerStatisticsUI statisticsUI = _uiInstance.GetComponentInChildren<PlayerStatisticsUI>();
        PlayerStats stats = GetComponent<PlayerStats>();

        if (statisticsUI != null && stats != null)
        {
            statisticsUI.Initialize(stats);
        }

        PlayerStaminaUI staminaUI = _uiInstance.GetComponentInChildren<PlayerStaminaUI>();
        PlayerController playerController = GetComponent<PlayerController>();

        if (staminaUI != null && playerController != null)
        {
            staminaUI.Initialize(playerController);
        }
    }

    private void OnDestroy()
    {
        if (isLocalPlayer && _uiInstance != null)
        {
            Destroy(_uiInstance);
        }
    }
}
