using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AmmoUI : MonoBehaviour
{
    private WeaponChanger _weaponChanger;
    private AmmoManager _ammoManager;
    private TextMeshProUGUI _ammoText;

    public void Initialize(WeaponChanger weaponChanger)
    {
        _ammoText = GetComponentInChildren<TextMeshProUGUI>();
        _weaponChanger = weaponChanger;

        if (_weaponChanger != null)
        {
            _weaponChanger.onWeaponChanged += SetAmmoUI;
        }
    }

    private void OnDisable()
    {
        if (_weaponChanger != null)
        {
            _weaponChanger.onWeaponChanged -= SetAmmoUI;
        }
        if (_ammoManager != null)
        {
            _ammoManager.onAmmoChanged -= UpdateAmmoUI;
        }
    }

    private void SetAmmoUI(List<GameObject> weapons, int index)
    {
        if (_ammoManager != null)
        {
            _ammoManager.onAmmoChanged -= UpdateAmmoUI;
        }

        _ammoManager = weapons[index].GetComponent<AmmoManager>();

        if (_ammoManager != null)
        {
            _ammoManager.onAmmoChanged += UpdateAmmoUI;
            UpdateAmmoUI(_ammoManager.CurrentAmmoInMag, _ammoManager.CurrentAmmoInReserve);
        }
    }

    private void UpdateAmmoUI(int ammoInMag, int ammoReserve)
    {
        if (_ammoText != null)
        {
            _ammoText.text = $"{ammoInMag}/{ammoReserve}";
        }
    }
}
