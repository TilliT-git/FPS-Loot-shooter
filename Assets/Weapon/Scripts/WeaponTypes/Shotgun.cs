using UnityEngine;

public class Shotgun : WeaponBase
{
    [SerializeField] protected int _countBullets;

    public override void HandleInput()
    {
        base.HandleInput();

        if (Input.GetMouseButtonDown(0) && CanShoot()) TryShoot();
    }

    //public override void BulletSpawn()
    //{
    //    for (int i = 0; i < _countBullets; i++)
    //    {
    //        base.BulletSpawn();
    //    }
    //}
}
