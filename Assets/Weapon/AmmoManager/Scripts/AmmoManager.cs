using Mirror;
using System;
using System.Collections;
using UnityEngine;

public class AmmoManager : NetworkBehaviour
{
    [SerializeField] private WeaponData _weaponData;

    private WeaponBase _weaponBase;

    [SyncVar(hook = nameof(OnCurrentAmmoChanged))]
    private int _currentAmmoInReserve;
    public int CurrentAmmoInReserve => _currentAmmoInReserve;

    [SyncVar(hook = nameof(OnCurrentAmmoInMagChanged))]
    private int _currentAmmoInMag;
    public int CurrentAmmoInMag => _currentAmmoInMag;

    [SyncVar]
    private bool _isReload;
    public bool IsReload => _isReload;

    private Coroutine _reloadCoroutine;

    public Action<int, int> onAmmoChanged;

    private void Awake()
    {
        _weaponBase = GetComponent<WeaponBase>();
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        _currentAmmoInReserve = _weaponData.MaxAmmoInReserve;
        _currentAmmoInMag = _weaponData.MaxAmmoInMag;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        AmmoChanged();
    }

    private void OnEnable()
    {
        if (_weaponBase != null)
        {
            _weaponBase.onShoot += RemoveAmmo;
            _weaponBase.onReload += StartReload;
        }
    }

    private void OnDisable()
    {
        if (_weaponBase != null)
        {
            _weaponBase.onShoot -= RemoveAmmo;
            _weaponBase.onReload -= StartReload;
        }
    }

    private void OnCurrentAmmoChanged(int oldAmmo, int newAmmo) => AmmoChanged();
    private void OnCurrentAmmoInMagChanged(int oldMag, int newMag) => AmmoChanged();

    private void RemoveAmmo()
    {
        if (_currentAmmoInMag <= 0 || _isReload) return;
        CmdRemoveAmmo();
    }

    [Command]
    private void CmdRemoveAmmo()
    {
        if (_currentAmmoInMag > 0 && !_isReload)
        {
            _currentAmmoInMag--;
        }
        else
        {
            Debug.Log("No ammo");
        }
    }

    private void StartReload()
    {
        if (_currentAmmoInMag < _weaponData.MaxAmmoInMag && _currentAmmoInReserve > 0 && !_isReload)
        {
            CmdStartReload();
        }
    }

    [Command]
    private void CmdStartReload()
    {
        if (_currentAmmoInMag < _weaponData.MaxAmmoInMag && _currentAmmoInReserve > 0 && !_isReload)
        {
            _reloadCoroutine = StartCoroutine(Reload());
        }
    }

    private IEnumerator Reload()
    {
        Debug.Log("Reload");

        _isReload = true;

        yield return new WaitForSeconds(_weaponData.ReloadTime);

        int ammoNeeded = _weaponData.MaxAmmoInMag - _currentAmmoInMag;

        if (_currentAmmoInReserve > ammoNeeded)
        {
            _currentAmmoInReserve -= ammoNeeded;
            _currentAmmoInMag += ammoNeeded;
        }
        else
        {
            _currentAmmoInMag += _currentAmmoInReserve;
            _currentAmmoInReserve = 0;
        }

        _isReload = false;
        _reloadCoroutine = null;

        Debug.Log("I'am reloaded");
    }

    private void AmmoChanged()
    {
        if (isLocalPlayer)
        {
            onAmmoChanged?.Invoke(_currentAmmoInMag, _currentAmmoInReserve);
        }
    }
}