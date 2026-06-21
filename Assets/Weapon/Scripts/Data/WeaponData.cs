using UnityEngine;

[CreateAssetMenu(fileName = "newWeapon", menuName = "Data/Weapons")]
public class WeaponData : ScriptableObject
{
    [Header("Effects")]
    [SerializeField] protected GameObject _decalPrefab;
    public GameObject DecalPrefab => _decalPrefab;
    [SerializeField] protected float _forceRecoilPos;
    public float ForceRecoilPos => _forceRecoilPos; 
    [SerializeField] protected float _forceVisualRecoil;
    public float ForceVisualRecoil => _forceVisualRecoil;
    [SerializeField] protected float _swayAmount;
    public float SwayAmount => _swayAmount;
    [SerializeField] protected float _maxSwayAmount;
    public float MaxSwayAmount => _maxSwayAmount;
    [SerializeField] protected float _swaySmoothing;
    public float SwaySmoothing => _swaySmoothing;
    [SerializeField] protected float _tiltAmount;
    public float TiltAmount => _tiltAmount;
    [SerializeField] protected float _maxTiltAmount;
    public float MaxTiltAmount => _maxTiltAmount;
    [SerializeField] protected float _tiltSmoothing;
    public float TiltSmoothing => _tiltSmoothing;
    [SerializeField] protected float _aimingFOV;
    public float AimingFOV => _aimingFOV;
    [SerializeField] protected float _aimingSpeed;
    public float AimingSpeed => _aimingSpeed;
    [SerializeField] protected float _aimingForceVisualRecoil;
    public float AimingForceVisualRecoil => _aimingForceVisualRecoil;

    [Header("Player Stats Change")]
    [SerializeField] protected float _walkSpeedMultiplier;
    public float WalkSpeedMultiplier => _walkSpeedMultiplier;
    [SerializeField] protected float _aimSpeedMultiplier;
    public float AimSpeedMultiplier => _aimSpeedMultiplier;

    [Header("Base Stats")]
    [SerializeField] protected int _maxAmmoInReserve;
    public int MaxAmmoInReserve => _maxAmmoInReserve;
    [SerializeField] protected int _maxAmmoInMag;
    public int MaxAmmoInMag => _maxAmmoInMag;
    [SerializeField] protected float _reloadTime;
    public float ReloadTime => _reloadTime;
    [SerializeField] protected float _fireRate;
    public float FireRate => _fireRate;
    [SerializeField] protected float _damageAmount;
    public float DamageAmount => _damageAmount;
    [SerializeField] protected float _maxDistance;
    public float MaxDistance => _maxDistance;
    [SerializeField] protected float _forceRecoilVerticalRot;
    public float ForceRecoilVerticalRot => _forceRecoilVerticalRot;
    [SerializeField] protected float _forceRecoilHorizontalRot;
    public float ForceRecoilHorizontalRot => _forceRecoilHorizontalRot;
    [SerializeField] protected float _spreadMultiplierX;
    public float SpreadMultiplierX => _spreadMultiplierX;
    [SerializeField] protected float _spreadMultiplierY;
    public float SpreadMultiplierY => _spreadMultiplierY;
    [SerializeField] protected float _aimingSpreadMultiplierX;
    public float AimingSpreadMultiplierX => _aimingSpreadMultiplierX;
    [SerializeField] protected float _aimingSpreadMultiplierY;
    public float AimingSpreadMultiplierY => _aimingSpreadMultiplierY;

    [Header("Aiming Pos")]
    [SerializeField] protected Vector3 _aimingPos;
    public Vector3 AimingPos => _aimingPos;
    [SerializeField] protected Vector3 _aimingRot;
    public Vector3 AimingRot => _aimingRot;
}