using Mirror;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class AmmoManager : NetworkBehaviour
{
    [SerializeField] protected int _maxAmmo;
    [SerializeField] protected int _ammoInMag;
    [SerializeField] protected float _reloadTime;

    private WeaponBase _weaponBase;

    [SyncVar(hook = nameof(OnCurrentAmmoChanged))]
    private int _currentAmmo;
    public int CurrentAmmo => _currentAmmo;

    [SyncVar(hook = nameof(OnCurrentAmmoInMagChanged))]
    private int _currentAmmoInMag;
    public int CurrentAmmoInMag => _currentAmmoInMag;

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
        _currentAmmo = _maxAmmo;
        _currentAmmoInMag = _ammoInMag;
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
        if (_currentAmmoInMag < _ammoInMag && CurrentAmmo > 0 && !_isReload)
        {
            CmdStartReload();
        }
    }

    [Command]
    private void CmdStartReload()
    {
        if (_currentAmmoInMag < _ammoInMag && CurrentAmmo > 0 && !_isReload)
        {
            _reloadCoroutine = StartCoroutine(Reload());
        }
    }

    private IEnumerator Reload()
    {
        Debug.Log("Reload");

        _isReload = true;

        yield return new WaitForSeconds(_reloadTime);

        int ammoNeeded = _ammoInMag - _currentAmmoInMag;

        if (_currentAmmo > ammoNeeded)
        {
            _currentAmmo -= ammoNeeded;
            _currentAmmoInMag += ammoNeeded;
        }
        else
        {
            _currentAmmoInMag += CurrentAmmo;
            _currentAmmo = 0;
        }

        _isReload = false;
        _reloadCoroutine = null;

        Debug.Log("I'am reloaded");
    }

    private void AmmoChanged()
    {
        if (isLocalPlayer)
        {
            onAmmoChanged?.Invoke(_currentAmmoInMag, _currentAmmo);
        }
    }
}