using UnityEngine;

public class RifleWeapon : WeaponBase
{
    [SerializeField] private Transform shootPoint;

    private PlayerAmmoSystem ammoSystem;
    private NoiseEmitter noiseEmitter;

    private void Awake()
    {
        ammoSystem = GetComponentInParent<PlayerAmmoSystem>();
        noiseEmitter = GetComponentInParent<NoiseEmitter>();
    }

    public override void Fire()
    {
        if (ammoSystem.CurrentAmmoGun <= 0)
            return;

        ammoSystem.DecreaseGunAmmo(data.ammoCost);

        Instantiate(data.projectilePrefab, shootPoint.position, shootPoint.rotation);

        noiseEmitter.MakeNoise(data.noiseRadius);

        AudioManager.Instance.PlaySFX(AudioManager.Instance.audioLibrary.playerGunShootSfx);
    }
}