using UnityEngine;
using UnityEngine.Pool;

public class ClusterBehavior : Weapon
{
    [Header("References")]
    [SerializeField]
    private ParticlesBehavior shootParticlesPrefab;

    [SerializeField]
    private Grenade grenadePrefab;

    [SerializeField]
    private Transform spawnPoint;

    [Header("Audio")]
    [SerializeField]
    private AudioClip grenadeShootSfx;

    private ObjectPool<ParticlesBehavior>
        shootParticlesPool;

    private ObjectPool<Grenade>
        grenadePool;

    // =====================================================
    // UNITY
    // =====================================================

    protected override void Awake()
    {
        base.Awake();

        shootParticlesPool =
            new ObjectPool<ParticlesBehavior>(
                OnCreateParticles,
                OnGetParticles,
                OnReleaseParticles
            );

        grenadePool =
            new ObjectPool<Grenade>(
                OnCreateGrenade,
                OnGetGrenade,
                OnReleaseGrenade
            );
    }

    // =====================================================
    // WEAPON
    // =====================================================

    public override void OnUse()
    {
        if (!HasAmmo())
            return;

        // AUDIO

        if (AudioManager.Instance != null &&
            grenadeShootSfx != null)
        {
            AudioManager.Instance.PlaySFX(
                grenadeShootSfx);
        }

        // AMMO

        ammoSystem.DecreaseClusterAmmo(1);

        // SHOOT

        grenadePool.Get();

        shootParticlesPool.Get();
    }

    public override bool HasAmmo()
    {
        return ammoSystem != null &&
               ammoSystem.CurrentAmmoCluster > 0;
    }

    // =====================================================
    // GRENADE POOL
    // =====================================================

    private Grenade OnCreateGrenade()
    {
        Grenade grenadeCopy =
            Instantiate(
                grenadePrefab,
                spawnPoint.position,
                Quaternion.identity);

        grenadeCopy.GrenadePool =
            grenadePool;

        return grenadeCopy;
    }

    private void OnGetGrenade(
        Grenade newGrenade)
    {
        Rigidbody rb =
            newGrenade.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        newGrenade.transform.position =
            spawnPoint.position;

        newGrenade.transform.rotation =
            spawnPoint.rotation;

        newGrenade.gameObject.SetActive(true);

        newGrenade.Launch(spawnPoint.forward);
    }

    private void OnReleaseGrenade(
        Grenade grenadeToRelease)
    {
        grenadeToRelease.gameObject.SetActive(false);
    }

    // =====================================================
    // PARTICLE POOL
    // =====================================================

    private ParticlesBehavior OnCreateParticles()
    {
        ParticlesBehavior particlesCopy =
            Instantiate(
                shootParticlesPrefab,
                spawnPoint.position,
                spawnPoint.rotation);

        particlesCopy.ParticlesPool =
            shootParticlesPool;

        return particlesCopy;
    }

    private void OnGetParticles(
        ParticlesBehavior newParticles)
    {
        newParticles.transform
            .SetPositionAndRotation(
                spawnPoint.position,
                spawnPoint.rotation);

        newParticles.gameObject.SetActive(true);

        newParticles.StartParticlesSystem();
    }

    private void OnReleaseParticles(
        ParticlesBehavior grenadeParticles)
    {
        grenadeParticles.gameObject.SetActive(false);
    }
}