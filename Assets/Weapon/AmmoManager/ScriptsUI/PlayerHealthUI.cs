using TMPro;
using UnityEngine;

public class PlayerHealthUI : MonoBehaviour
{
    private RectTransform _rectTransform;
    [SerializeField] private float _speedHealthBarAnim;

    private PlayerHealth _playerHealth;

    private Vector2 _targetScale;

    private void Awake()
    {
        if (_rectTransform == null)
        {
            _rectTransform = GetComponent<RectTransform>();
        }
        _targetScale = _rectTransform.localScale;
    }

    public void Initialize(PlayerHealth playerHealth)
    {
        _playerHealth = playerHealth;

        if (_playerHealth != null)
        {
            _playerHealth.onHealthChange += UpdateHealthUI;

            UpdateHealthUI(playerHealth.CurrentHealth, playerHealth.MaxHealth);
        }
    }

    private void OnDisable()
    {
        if (_playerHealth != null)
        {
            _playerHealth.onHealthChange -= UpdateHealthUI;
        }
    }

    private void Update()
    {
        if (_rectTransform != null)
        {
            _rectTransform.localScale = Vector2.Lerp(_rectTransform.localScale, _targetScale, _speedHealthBarAnim * Time.deltaTime);
        }
    }

    private void UpdateHealthUI(float currentHealth, float maxHealth)
    {
        if (maxHealth <= 0) return;

        float healthPercentage = Mathf.Clamp01(currentHealth / maxHealth);
        _targetScale = new Vector2(healthPercentage, _rectTransform.localScale.y);
    }
}
