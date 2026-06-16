using Cinemachine;
using Mirror;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    private CharacterController _characterController;

    [SerializeField] private Camera _mainCamera;
    private CinemachineVirtualCamera _virtualCamera;

    [SerializeField] private float _speed;
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _gravity;

    private float _currentVerticalVelocity;

    private float _horizontalInput;
    private float _verticalInput;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        _virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();

        if (isLocalPlayer)
        {
            if (_virtualCamera != null)
            {
                _virtualCamera.Follow = transform;
                _virtualCamera.LookAt = null;
            }
        }
        else
        {
            if (_virtualCamera != null)
            {
                _virtualCamera.gameObject.SetActive(false);
            }
        }
    }

    private void Update()
    {
        if (!isLocalPlayer) return;

        _horizontalInput = Input.GetAxis("Horizontal");
        _verticalInput = Input.GetAxis("Vertical");

        if (_characterController.isGrounded && Input.GetKeyDown(KeyCode.Space)) Jump();
    }

    private void FixedUpdate()
    {
        if (!isLocalPlayer) return;
        Movement();
    }

    private void Movement()
    {
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
        _characterController.Move(((finalMove * _speed) + velocityY) * Time.deltaTime);
    }

    private void Jump()
    {
        _currentVerticalVelocity = Mathf.Sqrt(_jumpForce * -2f * _gravity);
        Vector3 velocityY = new Vector3(0f, _currentVerticalVelocity, 0f);
        _characterController.Move(velocityY * Time.deltaTime);
    }
}