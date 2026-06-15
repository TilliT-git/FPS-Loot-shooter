using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class AmmoManager : MonoBehaviour
{
    [SerializeField] private int _maxAmmo;
    [SerializeField] public int _ammoInMag;
    [SerializeField] private int _reloadTime;

    private WeaponBase _weaponBase;
    private TextMeshProUGUI _ammoTextUI;

    public int CurrentAmmo { get; private set; }
    public int CurrentAmmoInMag { get; private set; }

    private bool _isReload;
    public bool IsReload => _isReload;

    private Coroutine _reloadCoroutine;

    public Action<int, int> onAmmoChanged;

    private void Awake()
    {
        CurrentAmmo = _maxAmmo;
        CurrentAmmoInMag = _ammoInMag;

        _weaponBase = GetComponent<WeaponBase>();
    }

    private void OnEnable()
    {
        _weaponBase.onShoot += RemoveAmmo;
        _weaponBase.onReload += StartReload;
        AmmoChanged();
    }

    private void OnDisable()
    {
        _weaponBase.onShoot -= RemoveAmmo;
        _weaponBase.onReload -= StartReload;
    }

    private void RemoveAmmo()
    {
        if (CurrentAmmoInMag > 0 && !_isReload)
        {
            CurrentAmmoInMag--;
            AmmoChanged();
        }
        else
        {
            Debug.Log("No ammo");
        }
    }

    private void StartReload()
    {
        if (CurrentAmmoInMag < _ammoInMag && CurrentAmmo > 0 && !_isReload)
        {
            _reloadCoroutine = StartCoroutine(Reload());
        }
    }

    private IEnumerator Reload()
    {
        Debug.Log("Reload");

        _isReload = true;

        yield return new WaitForSeconds(_reloadTime);

        int ammoNeeded = _ammoInMag - CurrentAmmoInMag;

        if (CurrentAmmo > ammoNeeded)
        {
            CurrentAmmo -= ammoNeeded;
            CurrentAmmoInMag += ammoNeeded;
        }
        else
        {
            CurrentAmmoInMag += CurrentAmmo;
            CurrentAmmo = 0;
        }

        AmmoChanged();
        _isReload = false;
        _reloadCoroutine = null;

        Debug.Log("I'am reloaded");
    }

    private void AmmoChanged()
    {
        onAmmoChanged?.Invoke(CurrentAmmoInMag, CurrentAmmo);
    }
}
