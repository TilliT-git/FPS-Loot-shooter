using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerController Instance { get; private set; }

    private CharacterController _characterController;

    [SerializeField] private Camera _mainCamera;

    [SerializeField] private float _speed;
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _gravity;
    [SerializeField] private float _mass;

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

        if (Input.GetKeyDown(KeyCode.Space)) Jump();
    }

    private void FixedUpdate()
    {
        Gravity();
        Move();
    }

    private void Move()
    {
        float horizontal = _horizontalInput;
        float vertical = _verticalInput;

        Vector3 horizontalMove = transform.right * horizontal;
        Vector3 verticalMove = transform.forward * vertical;

        Vector3 finalMove = horizontalMove + verticalMove;

        _characterController.Move(finalMove.normalized * _speed * Time.deltaTime);
    }

    private void Gravity()
    {
        Vector3 velocityY = new Vector3(0f, _gravity, 0f);
        _currentVerticalVelocity += _gravity * Time.deltaTime;
        Vector3 finalVelocityY = new Vector3(0f, _currentVerticalVelocity, 0f);
        _characterController.Move(finalVelocityY * Time.deltaTime);
    }

    private void Jump()
    {
        Debug.Log("JUMP");
        _currentVerticalVelocity = Mathf.Sqrt(_jumpForce * -2f * _gravity);
        Vector3 velocityY = new Vector3(0f, _currentVerticalVelocity, 0f);
        _characterController.Move(velocityY * Time.deltaTime);
    }
}