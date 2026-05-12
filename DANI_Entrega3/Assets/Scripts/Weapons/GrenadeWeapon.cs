using UnityEngine;

public class GrenadeWeapon : Weapon
{
    [SerializeField] private Grenade grenadePrefab;

    public override void OnUse()
    {
        if (!HasAmmo()) return;

        ammoSystem.DecreaseClusterAmmo(1);

        Grenade grenade = Instantiate(
            grenadePrefab,
            shootPoint.position,
            shootPoint.rotation
        );

        grenade.Launch(shootPoint.forward);
    }

    public override bool HasAmmo()
    {
        return ammoSystem.CurrentAmmoCluster > 0;
    }
}