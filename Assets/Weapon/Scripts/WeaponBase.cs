using System;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    public AmmoManager _ammoManager;

    [SerializeField] protected GameObject _decal;

    [SerializeField] protected float _swayAmount;
    [SerializeField] protected float _maxSwayAmount;
    [SerializeField] protected float _swaySmoothing;
    [SerializeField] protected float _tiltAmount;
    [SerializeField] protected float _maxTiltAmount;
    [SerializeField] protected float _tiltSmoothing;

    [SerializeField] protected float _forceRecoilPos;
    [SerializeField] protected float _forceRecoilRot;
    [SerializeField] protected float _fireRate;
    [SerializeField] protected float _rayOffset;
    [SerializeField] protected float _maxDistance;
    [SerializeField] protected float _spreadMultiplierY;
    [SerializeField] protected float _spreadMultiplierX;

    private float _fireRateTimer;

    private Vector3 _startPos;
    private Vector3 _currentPos;
    [SerializeField] private Vector3 _aimingPos;

    private Vector3 _startRot;
    private Vector3 _currentRot;
    [SerializeField] private Vector3 _aimingRot;

    private Vector3 _targetPos;
    private Vector3 _targetRot;

    private Vector3 _swayPos;
    private Vector3 _swayRot;

    public Action onShoot;
    public Action onReload;

    private void Start()
    {
        _ammoManager = GetComponent<AmmoManager>();

        _startPos = transform.localPosition;
        _startRot = transform.localEulerAngles;
        _targetPos = _startPos;
        _targetRot = _startRot;
    }

    private void Update()
    {
        HandleInput();
        WeaponSway();
        VisualRecoil();
        ShotDelay();

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
        _currentPos = Vector3.Lerp(_currentPos, _startPos, 5f * Time.deltaTime);
        _currentRot = Vector3.Lerp(_currentRot, _startRot, 5f * Time.deltaTime);
    }    

    public virtual void TryShoot()
    {
        _fireRateTimer = _fireRate;
        BulletSpawn();
        Recoil();
        onShoot?.Invoke();
    }

    public Vector3 CastSingleRay()
    {
        Vector2 randomPoint = UnityEngine.Random.insideUnitCircle;
        
        Vector3 localSpread = (transform.right * (randomPoint.y * _spreadMultiplierY)) + (transform.up * (randomPoint.x * _spreadMultiplierX));

        return localSpread;
    }

    public virtual bool CanShoot()
    {
        return !_ammoManager.IsReload && _ammoManager.CurrentAmmoInMag > 0 && _fireRateTimer <= 0;
    }

    public virtual void BulletSpawn()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position + transform.forward * _rayOffset, transform.forward + CastSingleRay());

        if (Physics.Raycast(ray, out hit) && hit.collider.gameObject != null)
        {
            if (hit.collider.gameObject.TryGetComponent<PlayerHealth>(out PlayerHealth playerHealth))
            {
                playerHealth.TakeDamage(10f);
            }

            Vector3 decalPos = hit.point + (hit.normal * 0.005f);
            Quaternion decalRot = Quaternion.LookRotation(-hit.normal);
            GameObject decal = Instantiate(_decal, decalPos, decalRot);
            decal.transform.SetParent(hit.collider.transform);
            Debug.Log($"POPAL B: {hit.collider.name}");
        }
    }

    private void TryAim()
    {
        if (Input.GetMouseButton(1))
        {
            _targetPos = Vector3.Lerp(_targetPos, _aimingPos, 5f * Time.deltaTime);
            _targetRot = Vector3.Lerp(_targetRot, _aimingRot, 5f * Time.deltaTime);
        }
        else
        {
            _targetPos = Vector3.Lerp(_targetPos, _startPos, 5f * Time.deltaTime);
            _targetRot = Vector3.Lerp(_targetRot, _startRot, 5f * Time.deltaTime);
        }
    }

    private void ShotDelay()
    {
        _fireRateTimer -= Time.deltaTime;
    }

    private void Recoil()
    {
        _currentPos.z += _forceRecoilPos;
        _currentRot.x += _forceRecoilRot;
    }

    private void TryReload()
    {
        onReload?.Invoke();
    }
}