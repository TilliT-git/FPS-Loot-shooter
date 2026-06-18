using Mirror;
using UnityEngine;

public class CameraController : NetworkBehaviour
{
    private Camera _playerCamera;

    [SerializeField] private float _returnSpeed = 10f;
    [SerializeField] private float _snaptiness = 15f;

    private float _xRotation;

    private Vector3 _targetRecoilRotation;
    private Vector3 _currentRecoilRotation;

    private void Start()
    {
        if (!isLocalPlayer) return;

        _playerCamera = GetComponentInChildren<Camera>();

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (!isLocalPlayer) return;

        CameraRotate();
    }

    private void CameraRotate()
    {
        if (_playerCamera == null) return;

        _targetRecoilRotation = Vector3.Lerp(_targetRecoilRotation, Vector3.zero, _returnSpeed * Time.deltaTime);
        _currentRecoilRotation = Vector3.Lerp(_currentRecoilRotation, _targetRecoilRotation, _snaptiness * Time.deltaTime);

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        transform.Rotate(Vector3.up * (mouseX + _currentRecoilRotation.y * Time.deltaTime * 50f));

        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -80f, 80f);

        _playerCamera.transform.localRotation = Quaternion.Euler(_xRotation + _currentRecoilRotation.x, _currentRecoilRotation.y, 0f);
    }

    public void AddRecoilCamera(float recoilX, float recoilY)
    {
        if (!isLocalPlayer) return;

        _targetRecoilRotation += new Vector3(-recoilX, recoilY, 0f);
    }
}
