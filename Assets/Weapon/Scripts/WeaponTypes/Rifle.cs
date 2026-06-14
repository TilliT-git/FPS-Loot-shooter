using UnityEngine;

public class Rifle : WeaponBase
{
    public override void HandleInput()
    {
        base.HandleInput();

        if (Input.GetMouseButton(0) && CanShoot()) TryShoot();
    }

    public override void BulletSpawn()
    {
        base.BulletSpawn();
    }
}