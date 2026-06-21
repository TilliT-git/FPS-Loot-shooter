using Mirror;
using System;
using System.Collections;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour
{
    private PlayerStats _playerStats;
    private CharacterController _characterController;

    private float _maxHealth;
    public float MaxHealth => _maxHealth;

    [SyncVar(hook = nameof(OnHealthChangedHook))]
    private float _currentHealth;
    public float CurrentHealth => _currentHealth;

    [SyncVar]
    private bool _isDeath = false;
    public bool IsDeath => _isDeath;

    [SerializeField] private GameObject _playerModel;
    [SerializeField] private float _respawnDelay = 3f;

    public Action <float, float> onHealthChange;

    private void Awake()
    {
        _playerStats = GetComponent<PlayerStats>();
        _characterController = GetComponent<CharacterController>();

        _maxHealth = _playerStats.MaxHealth;
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        _currentHealth = _maxHealth;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        HealthChange(_currentHealth, _maxHealth);
    }

    private void OnHealthChangedHook(float oldHealth, float newHealth)
    {
        HealthChange(newHealth, _maxHealth);
    }

    [Server]
    public void TakeDamage(float damageAmount)
    {
        if (_isDeath) return;

        _currentHealth -= damageAmount;

        if (_currentHealth <= 0f)
        {
            _currentHealth = 0f;
            ServarDeath();
        }
    }

    [Server]
    private void ServarDeath()
    {
        StartCoroutine(RespawnSequence());
    }

    [Server]
    private IEnumerator RespawnSequence()
    {
        RpcTogglePlayerState(false);

        if (NetworkManager.startPositions.Count > 0)
        {
            Transform randomSpawnPoint = NetworkManager.startPositions[UnityEngine.Random.Range(0, NetworkManager.startPositions.Count)];

            RcpTeleport(randomSpawnPoint.position, randomSpawnPoint.rotation);
        }

        yield return new WaitForSeconds(_respawnDelay);

        _currentHealth = _maxHealth;
        _isDeath = false;

        RpcTogglePlayerState(true);
    }

    [ClientRpc]
    private void RpcTogglePlayerState(bool isAlive)
    {
        if (_characterController != null) _characterController.enabled = isAlive;

        if (_playerModel != null) _playerModel.SetActive(isAlive);

        if (_playerModel.GetComponent<Collider>() != null) _playerModel.GetComponent<Collider>().enabled = isAlive;
    }

    [ClientRpc]
    private void RcpTeleport(Vector3 newPos, Quaternion newRot)
    {
        if (_characterController != null) _characterController.enabled = false;

        transform.position = newPos;
        transform.rotation = newRot;

        if (_characterController != null) _characterController.enabled = true;
    }

    private void HealthChange(float currentHealth, float maxHealth)
    {
        onHealthChange?.Invoke(currentHealth, maxHealth);
    }
}