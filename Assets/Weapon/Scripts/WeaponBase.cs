using Mirror;
using System;
using UnityEngine;

public abstract class WeaponBase : NetworkBehaviour
{
    public AmmoManager _ammoManager;
    private Camera _camera;
    private CameraController _cameraController;

    [SerializeField] protected GameObject _decal;

    [SerializeField] protected float _aimingFOV;
    [SerializeField] protected float _swayAmount;
    [SerializeField] protected float _maxSwayAmount;
    [SerializeField] protected float _swaySmoothing;
    [SerializeField] protected float _tiltAmount;
    [SerializeField] protected float _maxTiltAmount;
    [SerializeField] protected float _tiltSmoothing;

    [SerializeField] protected float _forceRecoilPos;
    [SerializeField] protected float _forceRecoilVerticalRot;
    [SerializeField] protected float _forceRecoilHorizontalRot;
    [SerializeField] protected float _fireRate;
    [SerializeField] protected float _damageAmount;
    [SerializeField] protected float _maxDistance;
    [SerializeField] protected float _spreadMultiplierX;
    [SerializeField] protected float _spreadMultiplierY;
    [SerializeField] protected float _aimingSpreadMultiplierX;
    [SerializeField] protected float _aimingSpreadMultiplierY;
    [SerializeField] protected float _aimingSpeed;

    private float _fireRateTimer;

    private Vector3 _startPos;
    private Vector3 _currentPos;
    [SerializeField] private Vector3 _aimingPos;

    private Vector3 _startRot;
    private Vector3 _currentRot;
    [SerializeField] private Vector3 _aimingRot;

    private Vector3 _targetPos;
    private Vector3 _targetRot;

    private float _startFOV = 60f;
    private float _targetFOV;

    private Vector3 _swayPos;
    private Vector3 _swayRot;

    private bool _isAiming;

    public Action onShoot;
    public Action onReload;

    private void Start()
    {
        _ammoManager = GetComponent<AmmoManager>();
        _camera = GetComponentInParent<Camera>();
        _cameraController = GetComponentInParent<CameraController>();

        if (!isLocalPlayer) return;

        _startPos = transform.localPosition;
        _startRot = transform.localEulerAngles;
        _targetPos = _startPos;
        _targetRot = _startRot;
    }

    private void Update()
    {
        if (!isLocalPlayer) return;

        HandleInput();
        WeaponSway();
        VisualRecoil();
        ShotDelay();

        Debug.DrawRay(_camera.transform.position, _camera.transform.forward * 100f);


        transform.localPosition = _targetPos + _currentPos + _swayPos;
        transform.localRotation = Quaternion.Euler(_targetRot + _currentRot + _swayRot);
    }

    public virtual void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.R)) TryReload();
        TryAim();
    }

    private void WeaponSway()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        float swayX = -mouseX * _swayAmount;
        float swayY = -mouseY * _swayAmount;

        Vector3 targetSway = new Vector3(swayX, swayY, 0f);
        _swayPos = Vector3.Lerp(_swayPos, targetSway, _swaySmoothing * Time.deltaTime);

        float tiltX = mouseY * _tiltAmount;
        float tiltY = -mouseX * _tiltAmount;
        float tiltZ = -mouseX * _tiltAmount * 0.5f;

        Vector3 targetTilt = new Vector3(tiltX, tiltY, tiltZ);
        _swayRot = Vector3.Lerp(_swayRot, targetTilt, _tiltSmoothing * Time.deltaTime);
    }

    private void VisualRecoil()
    {
        if (_isAiming)
        {
            _currentRot = Vector3.Lerp(_currentRot * (_aimingSpreadMultiplierX * _aimingSpreadMultiplierY), _startRot, 10f * Time.deltaTime);
        }
        else
        {
            _currentRot = Vector3.Lerp(_currentRot, _startRot, 5f * Time.deltaTime);
        }

        _currentPos = Vector3.Lerp(_currentPos, _startPos, 5f * Time.deltaTime);
    }    

    public virtual void TryShoot()
    {
        _fireRateTimer = _fireRate;
        Recoil();
        BulletSpawn();
        onShoot?.Invoke();
    }

    public Vector3 CastSingleRay()
    {
        Vector2 randomPoint = UnityEngine.Random.insideUnitCircle;

        float spreadX = randomPoint.x * _spreadMultiplierX * _aimingSpreadMultiplierX;
        float spreadY = randomPoint.y * _spreadMultiplierY * _aimingSpreadMultiplierY;

        Vector3 spreadDir = new Vector3(spreadY, spreadX, 0f);

        return spreadDir;
    }

    public virtual bool CanShoot()
    {
        return !_ammoManager.IsReload && _ammoManager.CurrentAmmoInMag > 0 && _fireRateTimer <= 0;
    }

    public virtual void BulletSpawn()
    {
        Vector3 targetDir = _camera.transform.forward;
        targetDir += CastSingleRay();
        targetDir.Normalize();
        Vector3 rayStartOrigin = _camera.transform.position + _camera.transform.forward;

        RaycastHit hit;
        Ray ray = new Ray(rayStartOrigin, targetDir);

        if (Physics.Raycast(ray, out hit) && hit.collider.gameObject != null)
        {
            PlayerHealth playerHealth = hit.collider.gameObject.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                CmdDamage(playerHealth, _damageAmount);
            }

            Vector3 decalPos = hit.point + (hit.normal * 0.001f);
            Quaternion decalRot = Quaternion.LookRotation(-hit.normal);
            GameObject decal = Instantiate(_decal, decalPos, decalRot);
            decal.transform.SetParent(hit.collider.transform);
            Debug.Log($"POPAL B: {hit.collider.name}");
        }
    }

    [Command]
    private void CmdDamage(PlayerHealth playerHealth, float damageAmount)
    {
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damageAmount);
        }
    }

    private void TryAim()
    {
        if (Input.GetMouseButton(1))
        {
            _targetPos = Vector3.Lerp(_targetPos, _aimingPos, 5f * Time.deltaTime);
            _targetRot = Vector3.Lerp(_targetRot, _aimingRot, 5f * Time.deltaTime);
            _targetFOV = Mathf.Lerp(_targetFOV, _aimingFOV, _aimingSpeed * Time.deltaTime);
            _isAiming = true;
        }
        else
        {
            _targetPos = Vector3.Lerp(_targetPos, _startPos, 5f * Time.deltaTime);
            _targetRot = Vector3.Lerp(_targetRot, _startRot, 5f * Time.deltaTime);
            _targetFOV = Mathf.Lerp(_targetFOV, _startFOV, _aimingSpeed * Time.deltaTime);
            _spreadMultiplierX = 1f;
            _spreadMultiplierY = 1f;
            _isAiming = false;
        }

        _camera.fieldOfView = _targetFOV;
    }


    private void ShotDelay()
    {
        _fireRateTimer -= Time.deltaTime;
    }

    private void Recoil()
    {
        float randomVertical = UnityEngine.Random.Range(0f, _forceRecoilVerticalRot);
        _currentRot.x -= randomVertical;

        float randomHorizontal = UnityEngine.Random.Range(-_forceRecoilHorizontalRot, _forceRecoilHorizontalRot);
        _currentRot.y += randomHorizontal;

        if (_cameraController != null && _isAiming)
        {
            _cameraController.AddRecoilCamera(randomVertical * _aimingSpreadMultiplierY, randomHorizontal * _aimingSpreadMultiplierX);
            _currentPos.z += _forceRecoilPos * (_aimingSpreadMultiplierX * _spreadMultiplierY);
        }
        else
        {
            _cameraController.AddRecoilCamera(randomVertical, randomHorizontal);
            _currentPos.z += _forceRecoilPos;
        }
    }

    private void TryReload()
    {
        onReload?.Invoke();
    }
}