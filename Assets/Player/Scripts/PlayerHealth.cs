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

    private float _currentHealth;
    public float CurrentHealth => _currentHealth;

    [SyncVar(hook = nameof(OnDeathStatusChangedHook))]
    private bool _isDeath = false;
    public bool IsDeath => _isDeath;

    [SerializeField] private GameObject _playerModel;
    [SerializeField] private float _respawnDelay = 3f;

    public Action <float, float> onHealthChange;
    public Action onDeath;

    private void Start()
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
        _currentHealth = _maxHealth;
        HealthChange(_currentHealth, _maxHealth);
    }

    private void OnDeathStatusChangedHook(bool oldStatus, bool newStatus)
    {
        if (newStatus)
        {
            onDeath?.Invoke();
        }
    }

    [Server]
    public void TakeDamage(float damageAmount, GameObject attacker)
    {
        if (_isDeath) return;

        _currentHealth -= damageAmount;

        if (_currentHealth <= 0f)
        {
            _currentHealth = 0f;
            ServerDeath(attacker);
        }

        RpcSyncHealthFromServer(_currentHealth, _maxHealth);
    }

    [ClientRpc]
    private void RpcSyncHealthFromServer(float newHealth, float maxHealth)
    {
        _currentHealth = newHealth;
        HealthChange(newHealth, maxHealth);
    }

    [Server]
    private void ServerDeath(GameObject attacker)
    {
        _isDeath = true;

        if (_playerStats != null) Debug.Log("ADD DEATH");

        if (attacker != null && attacker != gameObject)
        {
            if (attacker.TryGetComponent<PlayerStats>(out var attakerStats))
            {
                Debug.Log("ADD KILL");
            }
        }

        StartCoroutine(RespawnSequence());
    }

    [Server]
    private IEnumerator RespawnSequence()
    {
        RpcTogglePlayerState(false);

        yield return new WaitForSeconds(_respawnDelay);

        if (NetworkManager.startPositions.Count > 0)
        {
            Transform randomSpawnPoint = NetworkManager.startPositions[UnityEngine.Random.Range(0, NetworkManager.startPositions.Count)];

            RpcTeleport(randomSpawnPoint.position, randomSpawnPoint.rotation);
        }

        yield return null;

        _currentHealth = _maxHealth;
        _isDeath = false;

        RpcTogglePlayerState(true);
    }

    [ClientRpc]
    private void RpcTogglePlayerState(bool isAlive)
    {
        if (_characterController != null) _characterController.enabled = isAlive;

        if (_playerModel != null) _playerModel.SetActive(isAlive);
    }

    [ClientRpc]
    private void RpcTeleport(Vector3 newPos, Quaternion newRot)
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