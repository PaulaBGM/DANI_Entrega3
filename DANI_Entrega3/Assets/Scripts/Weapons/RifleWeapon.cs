using UnityEngine;

public class RifleWeapon : Weapon
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private ParticleSystem muzzleFlash;

    [SerializeField] private float damage = 20f;
    [SerializeField] private float distance = 500f;

    public override void OnUse()
    {
        if (!HasAmmo()) return;

        ammoSystem.DecreaseGunAmmo(1);

        muzzleFlash.Play();

        Instantiate(
            bulletPrefab,
            shootPoint.position,
            shootPoint.rotation
        );

        if (Physics.Raycast(
            mainCamera.transform.position,
            mainCamera.transform.forward,
            out RaycastHit hit,
            distance))
        {
            if (hit.transform.TryGetComponent(
                out EnemyBones enemy))
            {
                enemy.ApplyDamage(damage);
            }
        }
    }

    public override bool HasAmmo()
    {
        return ammoSystem.CurrentAmmoGun > 0;
    }
}