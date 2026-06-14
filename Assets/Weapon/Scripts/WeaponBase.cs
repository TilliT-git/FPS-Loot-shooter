using System;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    public AmmoManager _ammoManager;

    [SerializeField] protected float _forceRecoil;
    [SerializeField] protected float _rayOffset;
    [SerializeField] protected float _maxDistance;
    [SerializeField] protected float _spreadMultiplierY;
    [SerializeField] protected float _spreadMultiplierX;

    private Vector3 _startPos;
    private Vector3 _currentPos;
    private Vector3 _startRot;
    private Vector3 _currentRot;

    public Action onShoot;
    public Action onReload;

    private void Start()
    {
        _ammoManager = GetComponent<AmmoManager>();

        _startPos = transform.localPosition;
        _startRot = transform.localEulerAngles;
    }

    private void Update()
    {
        HandleInput();

        if (Input.GetKeyDown(KeyCode.R)) TryReload();

        VisualRecoil();
    }

    public virtual void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.R)) TryReload();
    }

    private void VisualRecoil()
    {
        _currentPos = Vector3.Lerp(_currentPos, Vector3.zero, 5f * Time.deltaTime);
        _currentRot = Vector3.Lerp(_currentRot, Vector3.zero, 5f * Time.deltaTime);

        transform.localPosition = _startPos + _currentPos;
        transform.localRotation = Quaternion.Euler(_startRot + _currentRot);
    }    

    public virtual void TryShoot()
    {
        BulletSpawn();
        Recoil();
        onShoot?.Invoke();
    }

    public Vector3 CastSingleRay()
    {
        return new Vector3(UnityEngine.Random.insideUnitCircle.y * _spreadMultiplierY, UnityEngine.Random.insideUnitCircle.x * _spreadMultiplierX, 0f);
    }

    public virtual bool CanShoot()
    {
        return !_ammoManager.IsReload && _ammoManager.CurrentAmmoInMag > 0;
    }

    public virtual void BulletSpawn()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position + transform.forward * _rayOffset, transform.forward + CastSingleRay());

        if (Physics.Raycast(ray, out hit))
        {
            Debug.Log($"POPAL B: {hit.collider.name}");
        }
    }

    public void Recoil()
    {
        _currentPos.z += _forceRecoil;
        _currentRot.x += _forceRecoil * 20f;
    }

    private void TryReload()
    {
        onReload?.Invoke();
    }
}