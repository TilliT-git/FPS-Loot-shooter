using Mirror;
using System;
using UnityEngine;

public abstract class WeaponBase : NetworkBehaviour
{
    [SerializeField] public WeaponData weaponData;

    private AmmoManager _ammoManager;
    private Camera _camera;
    private CameraController _cameraController;

    private PlayerStats _playerStats;

    private float _fireRateTimer;
    private float _startFOV = 60f;
    private Vector3 _startPos;
    private Vector3 _currentPos;

    private Vector3 _startRot;
    private Vector3 _currentRot;

    private Vector3 _targetPos;
    private Vector3 _targetRot;

    private float _targetFOV;

    private Vector3 _swayPos;
    private Vector3 _swayRot;

    private bool _isAiming;
    public bool IsAiming => _isAiming;

    public Action onShoot;
    public Action onReload;

    private void Start()
    {
        _ammoManager = GetComponent<AmmoManager>();
        _cameraController = GetComponentInParent<CameraController>();
        _playerStats = GetComponentInParent<PlayerStats>();
        _camera = GetComponentInParent<Camera>();
    }

    private void Awake()
    {
        GameManager.onEndMatch += DisabledComponent;
        GameManager.onStartMatch += EnabledComponent;
    }

    private void OnDestroy()
    {
        GameManager.onEndMatch -= DisabledComponent;
        GameManager.onEndMatch -= EnabledComponent;
    }

    private void DisabledComponent()
    {
        enabled = false;
    }

    private void EnabledComponent()
    {
        enabled = true;
    }

    private void Update()
    {
        if (!isLocalPlayer) return;

        HandleInput();
        WeaponSway();
        VisualRecoil();
        ShotDelay();

        transform.localPosition = _targetPos + _currentPos + _swayPos;

        if (_isAiming)
        {
            transform.localRotation = Quaternion.Euler(_targetRot + (_currentRot * weaponData.AimingForceVisualRecoil) + _swayRot);
        }
        else
        {
            transform.localRotation = Quaternion.Euler(_targetRot + (_currentRot * weaponData.ForceVisualRecoil) + _swayRot);
        }
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

        float swayX = -mouseX * weaponData.SwayAmount;
        float swayY = -mouseY * weaponData.SwayAmount;

        Vector3 targetSway = new Vector3(swayX, swayY, 0f);
        _swayPos = Vector3.Lerp(_swayPos, targetSway, weaponData.SwaySmoothing * Time.deltaTime);

        float tiltX = mouseY * weaponData.TiltAmount;
        float tiltY = -mouseX * weaponData.TiltAmount;
        float tiltZ = -mouseX * weaponData.TiltAmount * 0.5f;

        Vector3 targetTilt = new Vector3(tiltX, tiltY, tiltZ);
        _swayRot = Vector3.Lerp(_swayRot, targetTilt, weaponData.TiltSmoothing * Time.deltaTime);
    }

    private void VisualRecoil()
    {
        if (_isAiming)
        {
            _currentRot = Vector3.Lerp(_currentRot, _startRot, 10f * Time.deltaTime);
        }
        else
        {
            _currentRot = Vector3.Lerp(_currentRot, _startRot, 5f * Time.deltaTime);
        }

        _currentPos = Vector3.Lerp(_currentPos, _startPos, 2f * Time.deltaTime);
    }    

    public virtual void TryShoot()
    {
        _fireRateTimer = weaponData.FireRate;
        Recoil();

        Vector3 rayOrigin = _camera.transform.position;
        Vector3 rayDirection = _camera.transform.forward;

        rayDirection += CastSingleRay();

        CmdShoot(rayOrigin, rayDirection);
        onShoot?.Invoke();
    }

    public Vector3 CastSingleRay()
    {
        Vector2 randomPoint = UnityEngine.Random.insideUnitCircle;

        float spreadX = randomPoint.x * weaponData.SpreadMultiplierX * weaponData.AimingSpreadMultiplierX;
        float spreadY = randomPoint.y * weaponData.AimingSpreadMultiplierY * weaponData.AimingSpreadMultiplierY;

        Vector3 spreadDir = new Vector3(spreadY, spreadX, 0f);

        return spreadDir;
    }

    public virtual bool CanShoot()
    {
        return !_ammoManager.IsReload && _ammoManager.CurrentAmmoInMag > 0 && _fireRateTimer <= 0;
    }

    [Command]
    private void CmdShoot(Vector3 origin, Vector3 direction)
    {
        GameObject attacker = connectionToClient.identity.gameObject;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, weaponData.MaxDistance))
        {
            PlayerController targetPlayer = hit.collider.GetComponentInParent<PlayerController>();

            if (targetPlayer != null)
            {
                if (targetPlayer.gameObject == attacker) return;

                PlayerHealth targetHealth = targetPlayer.GetComponent<PlayerHealth>();
                if (targetHealth != null)
                {
                    targetHealth.TakeDamage(weaponData.DamageAmount, attacker);
                }

                return;
            }

            NetworkIdentity hitIdentity = hit.collider.GetComponent<NetworkIdentity>();
            GameObject objectToSend = (hitIdentity != null) ? hit.collider.gameObject : null;

            RpcSpawnDecal(hit.point, hit.normal, objectToSend);
        }
    }

    [ClientRpc]
    private void RpcSpawnDecal(Vector3 point, Vector3 normal, GameObject hitObject)
    {
        if (weaponData == null || weaponData.DecalPrefab == null) return;

        if (hitObject != null && hitObject.GetComponentInParent<PlayerHealth>() != null) return;

        Vector3 decalPos = point + (normal * 0.001f);
        Quaternion decalRot = Quaternion.LookRotation(-normal);

        GameObject decal = Instantiate(weaponData.DecalPrefab, decalPos, decalRot);

        if (hitObject != null)
        {
            decal.transform.SetParent(hitObject.transform);
        }
    }

    private void TryAim()
    {
        if (Input.GetMouseButton(1))
        {
            _targetPos = Vector3.Lerp(_targetPos, weaponData.AimingPos, 10f * Time.deltaTime);
            _targetRot = Vector3.Lerp(_targetRot, weaponData.AimingRot, 10f * Time.deltaTime);
            _targetFOV = Mathf.Lerp(_targetFOV, weaponData.AimingFOV, weaponData.AimingSpeed * Time.deltaTime);
            _isAiming = true;
        }
        else
        {
            _targetPos = Vector3.Lerp(_targetPos, _startPos, 5f * Time.deltaTime);
            _targetRot = Vector3.Lerp(_targetRot, _startRot, 5f * Time.deltaTime);
            _targetFOV = Mathf.Lerp(_targetFOV, _startFOV, weaponData.AimingSpeed * Time.deltaTime);
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
        float randomVertical = UnityEngine.Random.Range(0f, weaponData.ForceRecoilVerticalRot);
        _currentRot.x -= randomVertical;

        float randomHorizontal = UnityEngine.Random.Range(-weaponData.ForceRecoilHorizontalRot, weaponData.ForceRecoilHorizontalRot);
        _currentRot.y += randomHorizontal;

        if (_cameraController != null && _isAiming)
        {
            _cameraController.AddRecoilCamera(randomVertical * weaponData.AimingSpreadMultiplierY, randomHorizontal * weaponData.AimingSpreadMultiplierX);
            _currentPos.z = Mathf.Lerp(_currentPos.z, weaponData.ForceRecoilPos * weaponData.AimingForceVisualRecoil, 100f * Time.deltaTime);
        }
        else
        {
            _cameraController.AddRecoilCamera(randomVertical, randomHorizontal);
            _currentPos.z = Mathf.Lerp(_currentPos.z, weaponData.ForceRecoilPos, 100f * Time.deltaTime);
        }
    }

    private void TryReload()
    {
        onReload?.Invoke();
    }
}