using UnityEngine;

public class RifleWeapon : WeaponBase
{
    [SerializeField]
    private Transform shootPoint;

    [SerializeField]
    private float shootDistance = 100f;

    [SerializeField]
    private LayerMask hitMask;

    [SerializeField] private PlayerAmmoSystem ammoSystem;

    [SerializeField] private NoiseEmitter noiseEmitter;

    [SerializeField] private Camera mainCamera;

    private void OnEnable()
    {
        if (ammoSystem == null)
        {
            ammoSystem = GetComponentInParent<PlayerAmmoSystem>();
        }

        if (noiseEmitter == null)
        {
            noiseEmitter =
                GetComponentInParent<NoiseEmitter>();
        }

        if (mainCamera == null)
        {
            mainCamera =
                Camera.main;
        }

        Debug.Log(
            $"RIFLE ENABLED | ammo={ammoSystem} | noise={noiseEmitter}");
    }

    public override void Fire()
    {
        if (ammoSystem == null)
        {
            Debug.LogError(
                "AMMO SYSTEM NULL");

            return;
        }

        if (noiseEmitter == null)
        {
            Debug.LogError(
                "NOISE EMITTER NULL");

            return;
        }

        if (mainCamera == null)
        {
            Debug.LogError(
                "MAIN CAMERA NULL");

            return;
        }

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