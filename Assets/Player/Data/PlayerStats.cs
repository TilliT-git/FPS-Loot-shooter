using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : NetworkBehaviour
{
    [SerializeField] private PlayerData _playerData;
    [SerializeField] private GameManager _gameManager;

    private PlayerController _playerController;
    private WeaponChanger _weaponChanger;
    private WeaponBase _currentWeapon;

    public Action <int, int> onPlayerStatsChange;
    public static Action <PlayerStats> onPlayerMadeKill;

    [SyncVar(hook = nameof(OnWeaponIndexChanged))]
    private int _currentWeaponIndex;

    [Header("Stats")]
    [SyncVar(hook = nameof(OnKillsChanged))] private int _kills = 0;
    [SyncVar(hook = nameof(OnDeathsChanged))] private int _deaths = 0;
    public int Kills => _kills;
    public int Deaths => _deaths;

    public float MaxHealth => _playerData != null ? _playerData.MaxHealth : 100f;
    public float MoveSpeed => _playerData != null ? _playerData.MoveSpeed : 1f;
    public float JumpForce => _playerData != null ? _playerData.JumpForce : 5f;
    public float SprintSpeedMultiplier => _playerData != null ? _playerData.SprintSpeedMultiplier : 1f;

    public float WalkSpeedMultiplier => _currentWeapon != null ? _currentWeapon.weaponData.WalkSpeedMultiplier : 1f;
    public float AimSpeedMultiplier => _currentWeapon != null ? _currentWeapon.weaponData.AimSpeedMultiplier : 1f;
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

    private void Start()
    {
        UpdateCurrentWeaponReference();
    }

    private void HandleWeaponChanged(List<GameObject> weapons, int index)
    {
        if (isLocalPlayer)
        {
            CmdUpdateWeaponIndex(index);
        }
    }

    [Command]
    private void CmdUpdateWeaponIndex(int index)
    {
        _currentWeaponIndex = index;

        UpdateCurrentWeaponReference();
    }

    private void OnWeaponIndexChanged(int oldIndex, int newIndex)
    {
        _currentWeaponIndex = newIndex;

        UpdateCurrentWeaponReference();
    }

    private void UpdateCurrentWeaponReference()
    {
        if (_weaponChanger != null && _weaponChanger.Weapons != null && _currentWeaponIndex < _weaponChanger.Weapons.Count)
        {
            _currentWeapon = _weaponChanger.Weapons[_currentWeaponIndex].GetComponentInChildren<WeaponBase>();
        }
    }

    [Server]
    public void AddKill()
    {
        _kills++;

        onPlayerMadeKill?.Invoke(this);
    }

    [Server]
    public void AddDeath()
    {
        _deaths++;
    }

    private void OnKillsChanged(int oldVal, int newVal)
    {
        if (isLocalPlayer)
        {
            onPlayerStatsChange?.Invoke(newVal, _deaths);
        }
    }
    private void OnDeathsChanged(int oldVal, int newVal)
    {
        if (isLocalPlayer)
        {
            onPlayerStatsChange?.Invoke(_kills, newVal);
        }
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