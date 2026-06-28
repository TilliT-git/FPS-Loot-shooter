using Mirror;
using System;
using System.Collections;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour
{
    private PlayerStats _playerStats;
    private CharacterController _characterController;
    private NetworkTransformReliable _networkTransform;

    private float _maxHealth;
    public float MaxHealth => _maxHealth;

    [SyncVar(hook = nameof(OnHealthChangedHook))]
    private float _currentHealth;
    public float CurrentHealth => _currentHealth;

    [SyncVar(hook = nameof(OnDeathStatusChangedHook))]
    private bool _isDeath = false;
    public bool IsDeath => _isDeath;

    [SerializeField] private GameObject _playerModel;
    [SerializeField] private float _respawnDelay = 3f;

    public Action <float, float> onHealthChange;
    public Action onDeath;

    Coroutine _respawnCoroutine;

    private void Awake()
    {
        _playerStats = GetComponent<PlayerStats>();
        _characterController = GetComponent<CharacterController>();
        _networkTransform = GetComponent<NetworkTransformReliable>();

        _maxHealth = _playerStats.MaxHealth;

        GameManager.onStartMatch += StartMatchRespawn;
    }

    private void OnDestroy()
    {
        GameManager.onStartMatch -= StartMatchRespawn;
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

        TogglePlayerState(!_isDeath);
    }

    private void OnHealthChangedHook(float oldHealth, float newHealth)
    {
        if (isLocalPlayer)
        {
            HealthChange(newHealth, _maxHealth);
        }
    }

    private void OnDeathStatusChangedHook(bool oldStatus, bool newStatus)
    {
        TogglePlayerState(!newStatus);

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
    }

    [Server]
    private void ServerDeath(GameObject attacker)
    {
        _isDeath = true;

        if (_playerStats != null) _playerStats.AddDeath();

        if (attacker != null && attacker != gameObject)
        {
            if (attacker.TryGetComponent<PlayerStats>(out var attakerStats))
            {
                attakerStats.AddKill();
            }
        }

        Respawn();
    }

    private void Respawn()
    {
        if (!isServer) return;

        if (_respawnCoroutine == null)
        {
            StartCoroutine(RespawnSequence(_respawnDelay));
        }
    }

    private void StartMatchRespawn()
    {
        if (!isServer) return;

        if (_respawnCoroutine == null && _characterController.enabled)
        {
            StartCoroutine(RespawnSequence(0f));
        }
    }

    private IEnumerator RespawnSequence(float respawnDelay)
    {
        yield return new WaitForSeconds(respawnDelay);

        Vector3 spawnPos = Vector3.zero;
        Quaternion spawnRot = Quaternion.identity;

        if (NetworkManager.startPositions.Count > 0)
        {
            Transform randomSpawnPoint = NetworkManager.startPositions[UnityEngine.Random.Range(0, NetworkManager.startPositions.Count)];
            spawnPos = randomSpawnPoint.position;
            spawnRot = randomSpawnPoint.rotation;
        }

        RpcTeleportClient(spawnPos, spawnRot);

        yield return new WaitForSeconds(0.15f);

        _currentHealth = _maxHealth;
        _isDeath = false;
        _respawnCoroutine = null;
    }

    [ClientRpc]
    private void RpcTeleportClient(Vector3 spawnPos, Quaternion spawnRot)
    {
        if (!isLocalPlayer) return;

        if (_characterController != null) _characterController.enabled = false;

        transform.position = spawnPos;
        transform.rotation = spawnRot;

        if (_characterController != null) _characterController.enabled = true;
    }

    private void TogglePlayerState(bool isAlive)
    {
        if (_characterController != null) _characterController.enabled = isAlive;

        if (_playerModel != null) _playerModel.SetActive(isAlive);
    }

    private void HealthChange(float currentHealth, float maxHealth)
    {
        onHealthChange?.Invoke(currentHealth, maxHealth);
    }
}