using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateManager : NetworkBehaviour
{
    private CharacterController _characterController;
    private PlayerController _playerController;
    private CameraController _cameraController;
    private WeaponChanger _weaponChanger;
    [SerializeField] private GameObject _playerModel;
    [SerializeField] private List<WeaponBase> _weapons;

    [SyncVar(hook = nameof(OnDeathStatusChangedHook))]
    private bool _isDeath = false;
    public bool IsDeath => _isDeath;

    private bool _isPaused = false;

    public static bool IsControlsActive { get; private set; }

    [Server]
    public void SetDeathStatus(bool value) => _isDeath = value;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _playerController = GetComponent<PlayerController>();
        _cameraController = GetComponent<CameraController>();
        _weaponChanger = GetComponentInChildren<WeaponChanger>();

        ScreenManager.onPaused += HandlePause;
        GameManager.onStartMatch += MatchStarted;
        GameManager.onEndMatch += MatchEnded;
    }

    private void OnDestroy()
    {
        ScreenManager.onPaused -= HandlePause;
        GameManager.onStartMatch -= MatchStarted;
        GameManager.onEndMatch -= MatchEnded;
    }

    private void Start()
    {
        if (!isLocalPlayer)
        {
            enabled = false;
            return;
        }

        SetEverythingActive(true);
    }

    private void OnDeathStatusChangedHook(bool oldStatus, bool newStatus)
    {
        SetPlayerAliveStatus(!newStatus);
    }

    private void HandlePause(bool isPaused)
    {
        _isPaused = isPaused;
        SetEverythingActive(!isPaused);
    }

    private void MatchStarted()
    {
        _isPaused = false;
        SetEverythingActive(true);
    }

    private void MatchEnded()
    {
        _isPaused = true;
        SetEverythingActive(false);
    }

    private void SetEverythingActive(bool isActive)
    {
        IsControlsActive = isActive;

        if (_isPaused)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        
        if (_playerController != null) _playerController.enabled = isActive;
        if (_cameraController != null) _cameraController.enabled = isActive;
        if (_weaponChanger != null) _weaponChanger.enabled = isActive;
        if (_weapons != null)
        {
            foreach (var weapon in _weapons)
            {
                weapon.enabled = isActive;
            }
        }

        if (TryGetComponent<CharacterController>(out CharacterController characterController))
        {
            characterController.enabled = isActive;
        }
    }

    public void SetPlayerAliveStatus(bool isAlive)
    {
        SetEverythingActive(isAlive);

        if (_playerModel != null) _playerModel.SetActive(isAlive);
    }

    public void RespawnLocalPlayer(Vector3 spawnPos, Quaternion spawnRot)
    {
        if (_characterController != null) _characterController.enabled = false;

        transform.position = spawnPos;
        transform.rotation = spawnRot;
    }
}
