using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AmmoUI : MonoBehaviour
{
    [SerializeField] private WeaponChanger _weaponChanger;

    private AmmoManager _ammoManager;
    private TextMeshProUGUI _ammoText;

    private void Start()
    {
        _ammoText = GetComponent<TextMeshProUGUI>();

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
        }

        UpdateAmmoUI(_ammoManager.CurrentAmmoInMag, _ammoManager.CurrentAmmo);
    }

    private void UpdateAmmoUI(int ammoInMag, int ammoReserve)
    {
        if (_ammoText != null)
        {
            _ammoText.text = $"{ammoInMag}/{ammoReserve}";
        }
    }
}
