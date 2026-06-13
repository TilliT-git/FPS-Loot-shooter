using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    private CharacterController _characterController;

    [SerializeField] private Camera _mainCamera;

    [SerializeField] private float _speed;
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _gravity;

    private float _currentVerticalVelocity;

    private float _horizontalInput;
    private float _verticalInput;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        _characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        transform.eulerAngles = new Vector3(0f, _mainCamera.transform.eulerAngles.y, 0f);

        _horizontalInput = Input.GetAxis("Horizontal");
        _verticalInput = Input.GetAxis("Vertical");

        if (_characterController.isGrounded && Input.GetKeyDown(KeyCode.Space)) Jump();
    }

    private void FixedUpdate()
    {
        Movement();
    }

    private void Movement()
    {
        float horizontal = _horizontalInput;
        float vertical = _verticalInput;

        Vector3 horizontalMove = transform.right * horizontal;
        Vector3 verticalMove = transform.forward * vertical;

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