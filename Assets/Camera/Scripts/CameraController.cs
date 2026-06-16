using Mirror;
using UnityEngine;

public class CameraController : NetworkBehaviour
{
    private Camera _playerCamera;

    private float _mouseX;
    private float _mouseY;

    private void Start()
    {
        if (!isLocalPlayer) return;
        
        _playerCamera = GetComponentInChildren<Camera>();
    }

    private void Update()
    {
        if (!isLocalPlayer) return;

        CameraRotate();
    }

    private void CameraRotate()
    {
        if (_playerCamera == null) return;

        float _mouseX = Input.GetAxis("Mouse X");
        float _mouseY = Input.GetAxis("Mouse Y");

        transform.Rotate(Vector3.up * _mouseX);

        _playerCamera.transform.localEulerAngles += new Vector3(-_mouseY, 0f, 0f);
    }
}
