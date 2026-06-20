using Mirror;
using System;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour
{
    private PlayerStats _playerStats;

    private float _maxHealth;
    public float MaxHealth => _maxHealth;

    [SyncVar(hook = nameof(OnHealthChangedHook))]
    private float _currentHealth;
    public float CurrentHealth => _currentHealth;

    public Action <float, float> onHealthChange;

    private void Start()
    {
        _playerStats = GetComponent<PlayerStats>();

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

        if (newHealth <= 0f)
        {
            Death();
        }
    }

    [Server]
    public void TakeDamage(float damageAmount)
    {
        if (_currentHealth > 0f)
        {
            _currentHealth -= damageAmount;

            if (_currentHealth <= 0f)
            {
                _currentHealth = 0f;
                Death();
            }
        }
    }

    private void Death()
    {
        _currentHealth = 0;
        Debug.Log("DEATH");
    }

    private void HealthChange(float currentHealth, float maxHealth)
    {
        onHealthChange?.Invoke(currentHealth, maxHealth);
    }
}