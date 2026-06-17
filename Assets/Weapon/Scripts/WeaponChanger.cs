using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;
using Mirror;

public class WeaponChanger : NetworkBehaviour
{
    [SerializeField] private float _timeChangeWeapon;
    [SerializeField] private List<GameObject> _weapons;

    private int _currentWeapon = 0;

    private Coroutine _changeWeaponCoroutine;

    public Action<List<GameObject>, int> onWeaponChanged;

    private void Start()
    {
        _weapons[0].SetActive(true);
        WeaponChanged();
    }

    private void Update()
    {
        if (!isLocalPlayer) return;

        ScrollMouseWeapon();
        KeyChangeWeapon();
    }

    private void ScrollMouseWeapon()
    {
        int previousWeapon = _currentWeapon;

        if (Input.mouseScrollDelta.y > 0)
        {
            _currentWeapon = (_currentWeapon + 1) % _weapons.Count;
            Debug.Log("SCROLL UP");
        }
        else if (Input.mouseScrollDelta.y < 0)
        {
            _currentWeapon--;
            if (_currentWeapon < 0)
            {
                _currentWeapon = _weapons.Count - 1;
            }
            Debug.Log("SCROLL DOWN");
        }

        if (previousWeapon != _currentWeapon)
        {
            StartChangeWeapon(previousWeapon);
        }
    }

    private void KeyChangeWeapon()
    {
        int previousWeapon = _currentWeapon;

        for (int i = 0; i < 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                if (i < _weapons.Count)
                {
                    _currentWeapon = i;
                }
                break;
            }
        }

        if (previousWeapon != _currentWeapon)
        {
            StartChangeWeapon(previousWeapon);
        }
    }

    private void StartChangeWeapon(int previousWeapon)
    {
        if (_changeWeaponCoroutine != null)
        {
            StopCoroutine(_changeWeaponCoroutine);
        }

        _changeWeaponCoroutine = StartCoroutine(ChangeWeapon(previousWeapon, _currentWeapon));
    }

    private IEnumerator ChangeWeapon(int previousWeapon, int currentWeapon)
    {
        _weapons[previousWeapon].SetActive(false);
        yield return new WaitForSeconds(_timeChangeWeapon);
        _weapons[currentWeapon].SetActive(true);
        _changeWeaponCoroutine = null;
        WeaponChanged();
    }

    private void WeaponChanged()
    {
        onWeaponChanged?.Invoke(_weapons, _currentWeapon);
    }
}