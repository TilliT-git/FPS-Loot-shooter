using UnityEngine;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] private PlayerHealth _playerHealth;
    [SerializeField] private float _speedHealthBarAnim;

    private RectTransform _rectTransform;

    private Vector2 _targetScale;

    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _targetScale = _rectTransform.localScale;
    }

    private void OnEnable()
    {
        _playerHealth.onHealthChange += UpdateHealthUI;
    }

    private void OnDisable()
    {
        _playerHealth.onHealthChange -= UpdateHealthUI;
    }

    private void Update()
    {
        _rectTransform.localScale = Vector2.Lerp(_rectTransform.localScale, _targetScale, _speedHealthBarAnim * Time.deltaTime);
    }

    private void UpdateHealthUI(float currentHealth, float maxHealth)
    {
        _targetScale.x = currentHealth / maxHealth;
        _targetScale = new Vector2(_targetScale.x, _rectTransform.localScale.y);
    }
}
