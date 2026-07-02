using Mirror;
using System;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    private CharacterController _characterController;
    private PlayerStats _playerStats;

    [SerializeField] private float _gravity;

    private float _currentVerticalVelocity;

    private float _horizontalInput;
    private float _verticalInput;

    private float _currentStamina;
    public float CurrentStamina => _currentStamina;
    private float _maxStamina;
    public float MaxStamina => _maxStamina;

    private bool _isSprint;
    public bool IsSprint => _isSprint;

    public static Action<float, float> onStaminaChange;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _playerStats = GetComponent<PlayerStats>();

        _maxStamina = _playerStats.Stamina;
        _currentStamina = _maxStamina;
    }

    private void Update()
    {
        if (!isLocalPlayer) return;
        if (_characterController == null || !_characterController.enabled) return;

        _horizontalInput = Input.GetAxis("Horizontal");
        _verticalInput = Input.GetAxis("Vertical");

        if (_characterController.isGrounded && Input.GetKeyDown(KeyCode.Space)) Jump();

        Sprint();
    }

    private void Sprint()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            _isSprint = true;
            if (_currentStamina > 0)
            {
                _currentStamina -= Time.deltaTime;
            }
        }
        else
        {
            _isSprint = false;
            if (_currentStamina < _maxStamina)
            {
                _currentStamina += Time.deltaTime;
            }
        }

        onStaminaChange?.Invoke(_currentStamina, _maxStamina);
    }

    private void FixedUpdate()
    {
        if (!isLocalPlayer) return;
        if (_characterController == null || !_characterController.enabled) return;

        Movement();
    }

    private void Movement()
    {
        if (!_characterController.enabled) return;

        Vector3 horizontalMove = transform.right * _horizontalInput;
        Vector3 verticalMove = transform.forward * _verticalInput;

        if (_characterController.isGrounded)
        {
            _currentVerticalVelocity = -2f;
        }
        else
        {
            _currentVerticalVelocity += _gravity * Time.deltaTime;
        }

        Vector3 velocityY = new Vector3(0f, _currentVerticalVelocity, 0f);
        Vector3 finalMove = (horizontalMove + verticalMove).normalized;
        _characterController.Move(((finalMove * _playerStats.CurrentMoveSpeed) + velocityY) * Time.deltaTime);
    }

    private void Jump()
    {
        if (!_characterController.enabled) return;

        _currentVerticalVelocity = Mathf.Sqrt(_playerStats.JumpForce * -2f * _gravity);
        Vector3 velocityY = new Vector3(0f, _currentVerticalVelocity, 0f);
        _characterController.Move(velocityY * Time.deltaTime);
    }
}