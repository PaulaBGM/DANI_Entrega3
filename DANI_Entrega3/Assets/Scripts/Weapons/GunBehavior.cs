using UnityEngine;
using UnityEngine.Pool;

public class GunBehavior : Weapon
{
    [Header("VFX")]
    [SerializeField] private ParticlesBehavior gunParticles;

    [SerializeField] private Transform spawnPoint;

    [Header("Damage")]
    [SerializeField] private float damageDistance = 500f;

    [SerializeField] private float damageAmount = 20f;

    [Header("Audio")]
    [SerializeField] private AudioClip shootSfx;

    private ObjectPool<ParticlesBehavior> gunParticlesPool;

    protected override void Awake()
    {
        base.Awake();

        gunParticlesPool =
            new ObjectPool<ParticlesBehavior>(
                OnCreateParticles,
                OnGetParticles,
                OnReleaseParticles
            );
    }

    public override void OnUse()
    {
        if (!HasAmmo())
            return;

        if (AudioManager.Instance != null &&
            shootSfx != null)
        {
            AudioManager.Instance.PlaySFX(shootSfx);
        }

        gunParticlesPool.Get();

        if (Physics.Raycast(
            mainCamera.transform.position,
            mainCamera.transform.forward,
            out RaycastHit hitInfo,
            damageDistance))
        {

            if (hitInfo.transform.TryGetComponent(
                out IDamagable damageable))
            {
                damageable.ApplyDamage(damageAmount);
            }
        }

        ammoSystem.DecreaseGunAmmo(1);
    }

    public override bool HasAmmo()
    {
        return ammoSystem != null &&
               ammoSystem.CurrentAmmoGun > 0;
    }

    private ParticlesBehavior OnCreateParticles()
    {
        ParticlesBehavior particlesCopy =
            Instantiate(
                gunParticles,
                spawnPoint.position,
                Quaternion.identity);

        particlesCopy.ParticlesPool =
            gunParticlesPool;

        return particlesCopy;
    }

    private void OnGetParticles(
        ParticlesBehavior newParticles)
    {
        newParticles.transform.SetParent(
            spawnPoint,
            false);

        newParticles.transform.localPosition =
            Vector3.zero;

        newParticles.transform.localRotation =
            Quaternion.identity;

        newParticles.gameObject.SetActive(true);

        newParticles.StartParticlesSystem();
    }

    private void OnReleaseParticles(
        ParticlesBehavior particles)
    {
        particles.gameObject.SetActive(false);
    }
}