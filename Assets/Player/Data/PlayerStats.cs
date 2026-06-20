using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private PlayerData _playerData;

    private PlayerController _playerController;
    private WeaponChanger _weaponChanger;
    private WeaponBase _currentWeapon;

    public float MaxHealth => _playerData != null ? _playerData.MaxHealth : 100f;
    public float MoveSpeed => _playerData != null ? _playerData.MoveSpeed : 1f;
    public float JumpForce => _playerData != null ? _playerData.JumpForce : 5f;
    public float SprintSpeedMultiplier => _playerData != null ? _playerData.SprintSpeedMultiplier : 1f;

    public float WalkSpeedMultiplier => _currentWeapon != null ? _currentWeapon.walkSpeedMultiplier : 1f;
    public float AimSpeedMultiplier => _currentWeapon != null ? _currentWeapon.aimSpeedMultiplier : 1f;
    public bool IsAiming => _currentWeapon != null ? _currentWeapon.IsAiming : false;

    public bool IsSprint => _playerController != null ? _playerController.IsSprint : false;

    private void Awake()
    {
        _playerController = GetComponent<PlayerController>();
        _weaponChanger = GetComponentInChildren<WeaponChanger>();
    }

    private void OnEnable()
    {
        if (_weaponChanger != null)
        {
            _weaponChanger.onWeaponChanged += HandleWeaponChanged;
        }
    }

    private void OnDisable()
    {
        if (_weaponChanger != null)
        {
            _weaponChanger.onWeaponChanged -= HandleWeaponChanged;
        }
    }

    private void HandleWeaponChanged(List<GameObject> weapons, int index)
    {
        _currentWeapon = weapons[index].GetComponentInChildren<WeaponBase>();
    }

    public float CurrentMoveSpeed
    {
        get
        {
            if (_currentWeapon == null) return MoveSpeed;

            if (IsAiming)
            {
                return MoveSpeed * WalkSpeedMultiplier * AimSpeedMultiplier;
            }
            else if (IsSprint)
            {
                return MoveSpeed * WalkSpeedMultiplier * SprintSpeedMultiplier;
            }
            else
            {
                return MoveSpeed * WalkSpeedMultiplier;
            }
        }
    }
}