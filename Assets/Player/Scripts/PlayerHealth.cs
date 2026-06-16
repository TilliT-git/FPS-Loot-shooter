using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] protected float _maxHealth;

    private float _currentHealth;

    public Action <float, float> onHealthChange;

    private void Awake()
    {
        _currentHealth = _maxHealth;
    }

    public void TakeDamage(float damageAmount)
    {
        if (_currentHealth > 0f)
        {
            _currentHealth -= damageAmount;
        }
        else
        {
            Death();
        }

        HealthChange(_currentHealth, _maxHealth);
    }

    private void Death()
    {
        _currentHealth = 0;
        Debug.Log("DEATH");
    }

    private void HealthChange(float curerntHealth, float maxHealth)
    {
        onHealthChange?.Invoke(_currentHealth, maxHealth);
    }
}