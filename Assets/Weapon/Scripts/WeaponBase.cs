using System;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    private AmmoManager _ammoManager;

    [SerializeField] private float _forceRecoil;
    [SerializeField] private float _rayOffset;
    [SerializeField] private float _maxDistance;

    private Vector3 _startPos;

    public Action onShoot;
    public Action onReload;

    private void Start()
    {
        _ammoManager = GetComponent<AmmoManager>();

        _startPos = transform.localPosition;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) TryReload();

        Ray ray = new Ray(transform.position + transform.forward * _rayOffset, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, _maxDistance) && Input.GetMouseButtonDown(0))
        {
            TryShoot();
            Debug.Log($"{hit.collider.gameObject.name}");
        }
        Debug.DrawRay(transform.position + transform.forward * _rayOffset, transform.forward * _maxDistance, Color.red);

        transform.localPosition = Vector3.Lerp(transform.localPosition, _startPos, 5f * Time.deltaTime);
    }

    private void TryShoot()
    {
        if (!_ammoManager.IsReload && _ammoManager.CurrentAmmoInMag > 0)
        {
            onShoot?.Invoke();
            Recoil();
        }
    }

    private void Recoil()
    {
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, _forceRecoil);
    }

    private void TryReload()
    {
        onReload?.Invoke();
    }
}