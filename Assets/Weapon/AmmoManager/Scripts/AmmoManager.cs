using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class AmmoManager : MonoBehaviour
{
    [SerializeField] private int _maxAmmo;
    [SerializeField] private int _maxAmmoInMag;
    [SerializeField] private int _reloadTime;

    private WeaponBase _weaponBase;
    private TextMeshProUGUI _ammoTextUI;

    private int _currentAmmo;
    private int _currentAmmoInMag;
    public int CurrentAmmoInMag => _currentAmmoInMag;

    private bool _isReload;
    public bool IsReload => _isReload;

    Coroutine _reloadCoroutine;

    public Action<int, int> onAmmoChanged;

    private void Awake()
    {
        _currentAmmo = _maxAmmo;
        _currentAmmoInMag = _maxAmmoInMag;

        _weaponBase = GetComponent<WeaponBase>();
        AmmoChanged();
    }

    private void OnEnable()
    {
        _weaponBase.onShoot += RemoveAmmo;
        _weaponBase.onReload += StartReload;
    }

    private void OnDisable()
    {
        _weaponBase.onShoot -= RemoveAmmo;
        _weaponBase.onReload -= StartReload;
    }

    private void RemoveAmmo()
    {
        if (_currentAmmoInMag > 0 && !_isReload)
        {
            _currentAmmoInMag--;
            AmmoChanged();
        }
        else
        {
            Debug.Log("No ammo");
        }
    }

    private void StartReload()
    {
        if (_currentAmmoInMag < _maxAmmoInMag && _currentAmmo > 0 && !_isReload)
        {
            _reloadCoroutine = StartCoroutine(Reload());
        }
    }

    private IEnumerator Reload()
    {
        Debug.Log("Reload");

        _isReload = true;

        yield return new WaitForSeconds(_reloadTime);

        int ammoNeeded = _maxAmmoInMag - _currentAmmoInMag;

        if (_currentAmmo > ammoNeeded)
        {
            _currentAmmo -= ammoNeeded;
            _currentAmmoInMag += ammoNeeded;
        }
        else
        {
            _currentAmmoInMag += _currentAmmo;
            _currentAmmo = 0;
        }

        AmmoChanged();
        _isReload = false;
        _reloadCoroutine = null;

        Debug.Log("I'am reloaded");
    }

    private void AmmoChanged()
    {
        onAmmoChanged?.Invoke(_currentAmmoInMag, _currentAmmo);
    }
}
