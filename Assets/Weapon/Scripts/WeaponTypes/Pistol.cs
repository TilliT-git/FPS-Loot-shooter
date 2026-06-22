using UnityEngine;

public class Pistol : WeaponBase
{
    public override void HandleInput()
    {
        base.HandleInput();

        if (Input.GetMouseButtonDown(0) && CanShoot()) TryShoot();
    }

    public void BulletSpawn()
    {
        
    }
}