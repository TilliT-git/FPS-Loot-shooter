using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateManager : NetworkBehaviour
{
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private CameraController _cameraController;
    [SerializeField] private WeaponChanger _weaponChanger;
    [SerializeField] private List<WeaponBase> _weapons; 

    private void Awake()
    {
        ScreenManager.onPaused += HandlePause;
        GameManager.onStartMatch += MatchStarted;
        GameManager.onEndMatch += MatchEnded;
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

    private void OnDestroy()
    {
        ScreenManager.onPaused -= HandlePause;
        GameManager.onStartMatch -= MatchStarted;
        GameManager.onEndMatch -= MatchEnded;
    }

    private void HandlePause(bool isPaused) => SetEverythingActive(!isPaused);
    private void MatchStarted() => SetEverythingActive(true);
    private void MatchEnded() => SetEverythingActive(false);

    private void SetEverythingActive(bool isActive)
    {
        Cursor.visible = !isActive;
        Cursor.lockState = isActive ? CursorLockMode.Locked : CursorLockMode.None;
        
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
            Debug.Log("asd");
        }
    }
}
