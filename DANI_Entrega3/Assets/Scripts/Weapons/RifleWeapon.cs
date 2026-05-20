using UnityEngine;

public class RifleWeapon : WeaponBase
{
    [SerializeField]
    private Transform shootPoint;

    [SerializeField]
    private float shootDistance = 100f;

    [SerializeField]
    private LayerMask hitMask;

    private PlayerAmmoSystem ammoSystem;

    private NoiseEmitter noiseEmitter;

    private Camera mainCamera;

    private void Awake()
    {
        ammoSystem =
            GetComponentInParent<PlayerAmmoSystem>();

        noiseEmitter =
            GetComponentInParent<NoiseEmitter>();

        mainCamera =
            Camera.main;
    }

    public override void Fire()
    {
        if (ammoSystem.CurrentAmmoGun <= 0)
            return;

        ammoSystem.DecreaseGunAmmo(
            data.ammoCost);

        noiseEmitter.MakeNoise(
            data.noiseRadius);

        AudioManager.Instance.PlaySFX(
            AudioManager.Instance.audioLibrary.playerGunShootSfx);

        ShootRaycast();
    }

    // =========================
    // HITSCAN
    // =========================

    private void ShootRaycast()
    {
        Ray ray =
            new Ray(
                mainCamera.transform.position,
                mainCamera.transform.forward);

        if (Physics.Raycast(
            ray,
            out RaycastHit hit,
            shootDistance,
            hitMask))
        {
            Debug.Log(
                $"SHOT HIT: {hit.collider.name}");

            EnemyHealthSystem enemy =
                hit.collider.GetComponentInParent<EnemyHealthSystem>();

            if (enemy != null)
            {
                enemy.TakeDamage(
                    data.damage);

                Debug.Log(
                    $"ENEMY DAMAGED: {data.damage}");
            }
        }
    }
}