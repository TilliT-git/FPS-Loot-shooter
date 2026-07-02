using UnityEngine;

public class PlayerStaminaUI : MonoBehaviour
{
    private RectTransform _rectTransform;
    private PlayerController _playerController;

    [SerializeField] private float _speedStaminaBarAnim;

    private Vector2 _targetScale;

    private void Awake()
    {
        if (_rectTransform == null)
        {
            _rectTransform = GetComponent<RectTransform>();
        }
        _targetScale = _rectTransform.localScale;

        if (_playerController != null)
        {
            PlayerController.onStaminaChange += UpdateStaminaUI;
        }
    }

    public void Initialize(PlayerController playerController)
    {
        _playerController = playerController;

        if (_playerController != null)
        {
            PlayerController.onStaminaChange += UpdateStaminaUI;

            UpdateStaminaUI(_playerController.CurrentStamina, _playerController.MaxStamina);
        }
    }

    private void OnDisable()
    {
        if (_playerController != null)
        {
            PlayerController.onStaminaChange -= UpdateStaminaUI;
        }
    }

    private void Update()
    {
        if (_rectTransform != null)
        {
            _rectTransform.localScale = Vector2.Lerp(_rectTransform.localScale, _targetScale, _speedStaminaBarAnim * Time.deltaTime);
        }
    }

    private void UpdateStaminaUI(float currentStamina, float maxStamina)
    {
        if (maxStamina <= 0) return;

        float staminaPercentage = Mathf.Clamp01(currentStamina / maxStamina);
        _targetScale = new Vector2(staminaPercentage, _rectTransform.localScale.y);
    }
}
