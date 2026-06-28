using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;
using Mirror;

public class WeaponChanger : NetworkBehaviour
{
    [SerializeField] private float _timeChangeWeapon;
    [SerializeField] private List<GameObject> _weapons;
    public List<GameObject> Weapons => _weapons;

    [SyncVar(hook = nameof(OnWeaponIndexChanged))]
    private int _currentWeapon = 0;

    private Coroutine _changeWeaponCoroutine;
    private bool _isChanging = false;

    public Action<List<GameObject>, int> onWeaponChanged;

    private void Start()
    {
        InitWeaponsVisibility();
    }

    private void Awake()
    {
        GameManager.onEndMatch += DisabledComponent;
        GameManager.onStartMatch += EnabledComponent;
    }

    private void OnDestroy()
    {
        GameManager.onEndMatch -= DisabledComponent;
        GameManager.onStartMatch -= EnabledComponent;
    }

    private void DisabledComponent()
    {
        enabled = false;
    }

    private void EnabledComponent()
    {
        enabled = true;
    }

    private void Update()
    {
        if (!isLocalPlayer) return;
        if (_isChanging) return;

        ScrollMouseWeapon();
        KeyChangeWeapon();
    }

    private void ScrollMouseWeapon()
    {
        int targetWeapon = _currentWeapon;

        if (Input.mouseScrollDelta.y > 0)
        {
            targetWeapon = (_currentWeapon + 1) % _weapons.Count;
            Debug.Log("SCROLL UP");
        }
        else if (Input.mouseScrollDelta.y < 0)
        {
            targetWeapon--;
            if (targetWeapon < 0)
            {
                targetWeapon = _weapons.Count - 1;
            }
            Debug.Log("SCROLL DOWN");
        }

        if (targetWeapon != _currentWeapon)
        {
            _isChanging = true;
            CmdRequestWeaponChange(targetWeapon);
        }
    }

    private void KeyChangeWeapon()
    {
        int targetWeapon = _currentWeapon;

        for (int i = 0; i < 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                if (i < _weapons.Count)
                {
                    targetWeapon = i;
                }
                break;
            }
        }

        if (targetWeapon != _currentWeapon)
        {
            _isChanging = true;
            CmdRequestWeaponChange(targetWeapon);
        }
    }

    [Command]
    private void CmdRequestWeaponChange(int newIndex)
    {
        if (newIndex >= 0 && newIndex < _weapons.Count)
        {
            _currentWeapon = newIndex;
        }
    }

    private void OnWeaponIndexChanged(int oldWeapon, int newWeapon)
    {
        StartChangeWeapon(oldWeapon, newWeapon);
    }

    private void StartChangeWeapon(int previousWeapon, int targetWeapon)
    {
        if (_changeWeaponCoroutine != null)
        {
            StopCoroutine(_changeWeaponCoroutine);
        }

        _changeWeaponCoroutine = StartCoroutine(ChangeWeapon(previousWeapon, targetWeapon));
    }

    private IEnumerator ChangeWeapon(int previousWeapon, int targetWeapon)
    {
        _weapons[previousWeapon].SetActive(false);

        yield return new WaitForSeconds(_timeChangeWeapon);

        _weapons[targetWeapon].SetActive(true);
        _changeWeaponCoroutine = null;

        if (isLocalPlayer) _isChanging = false;

        WeaponChanged();
    }

    private void InitWeaponsVisibility()
    {
        for (int i = 0; i < _weapons.Count; i++)
        {
            if (_weapons[i] != null)
            {
                _weapons[i].SetActive(i == _currentWeapon);
            }
        }

        WeaponChanged();
    }

    private void WeaponChanged()
    {
        if (isLocalPlayer)
        {
            onWeaponChanged?.Invoke(_weapons, _currentWeapon);
        }
    }
}