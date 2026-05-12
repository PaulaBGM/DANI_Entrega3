using UnityEngine;

public class PistolWeapon : Weapon
{
    [SerializeField] private GameObject bulletPrefab;

    public override void OnUse()
    {
        if (!HasAmmo()) return;

        ammoSystem.DecreaseGunAmmo(1);

        Instantiate(
            bulletPrefab,
            shootPoint.position,
            shootPoint.rotation
        );
    }

    public override bool HasAmmo()
    {
        return ammoSystem.CurrentAmmoGun > 0;
    }
}