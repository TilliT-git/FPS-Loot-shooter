using Mirror;
using UnityEngine;

public class PlayerUISpawner : NetworkBehaviour
{
    [SerializeField] private GameObject _uiCanvasPrefab;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        GameObject uiInstance = Instantiate(_uiCanvasPrefab);

        AmmoUI ammoUI = uiInstance.GetComponentInChildren<AmmoUI>();
        WeaponChanger weaponChanger = GetComponentInChildren<WeaponChanger>();

        if (ammoUI != null && weaponChanger != null)
        {
            ammoUI.Initialize(weaponChanger);
        }

        PlayerHealthUI healthUI = uiInstance.GetComponentInChildren<PlayerHealthUI>();
        PlayerHealth health = GetComponent<PlayerHealth>();

        if (healthUI != null && health != null)
        {
            healthUI.Initialize(health);
        }
    }
}
