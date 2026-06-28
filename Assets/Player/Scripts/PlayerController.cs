using Mirror;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    private CharacterController _characterController;
    private PlayerStats _playerStats;

    [SerializeField] private Camera _mainCamera;

    [SerializeField] private float _gravity;

    private float _currentVerticalVelocity;

    private float _horizontalInput;
    private float _verticalInput;

    private bool _isSprint;
    public bool IsSprint => _isSprint;

    private void Start()
    {
        if (!isLocalPlayer)
        {
            this.enabled = false;

            Camera playerCam = GetComponentInChildren<Camera>();
            if (playerCam != null) playerCam.enabled = false;

            return;
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        _playerStats = GetComponent<PlayerStats>();
    }

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        GameManager.onEndMatch += DisabledComponent;
        GameManager.onStartMatch += EnabledComponent;
    }

    private void OnDestroy()
    {
        GameManager.onEndMatch -= DisabledComponent;
        GameManager.onStartMatch -= EnabledComponent;
    }

    private void DisabledComponent()
    {
        enabled = false;
    }

    private void EnabledComponent()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        enabled = true;
    }

    private void Update()
    {
        if (!isLocalPlayer) return;
        if (_characterController == null || !_characterController.enabled) return;

        _horizontalInput = Input.GetAxis("Horizontal");
        _verticalInput = Input.GetAxis("Vertical");

        if (_characterController.isGrounded && Input.GetKeyDown(KeyCode.Space)) Jump();

        if (Input.GetKey(KeyCode.LeftShift))
        {
            _isSprint = true;
        }
        else
        {
            _isSprint = false;
        }
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