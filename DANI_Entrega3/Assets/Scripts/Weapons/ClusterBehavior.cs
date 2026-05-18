using System;
using UnityEngine;
using UnityEngine.Pool;

public class ClusterBehavior : Weapons
{
    [SerializeField] private ParticlesBehavior shootParticlesPrefab;
    [SerializeField] private Grenade grenadePrefab;
    [SerializeField] private Transform spawnPoint;

    private ObjectPool<ParticlesBehavior> shootParticlesPool;
    private ObjectPool<Grenade> grenadePool;

    private void Awake()
    {
        shootParticlesPool = new ObjectPool<ParticlesBehavior>(OnCreateParticles, OnGetParticles, OnReleaseParticles);
        grenadePool = new ObjectPool<Grenade>(OnCreateGrenade, OnGetGrenade, OnReleaseGrenade);
        playerAmmoSystem = GetComponentInParent<PlayerAmmoSystem>();
    }

    private Grenade OnCreateGrenade() //Genera nuevas granadas cuando no hay disponibles.
    {
        Grenade grenadeCopy = Instantiate(grenadePrefab, spawnPoint.position, Quaternion.identity);
        grenadeCopy.GrenadePool = grenadePool;
        return grenadeCopy;
    }

    private void OnGetGrenade(Grenade newGrenade) //Para activar alguna de las granadas disponibles.
    {
        newGrenade.GetComponent<Rigidbody>().linearVelocity = Vector3.zero; //Se detiene la fuerza por la que es propulsada.
        newGrenade.transform.position = spawnPoint.position;
        newGrenade.transform.rotation = spawnPoint.rotation;
        newGrenade.gameObject.SetActive(true);

        newGrenade.Launch(spawnPoint.forward);
    }

    private void OnReleaseGrenade(Grenade grenadeToRelease) //Para desactivar las granadas
    {
        grenadeToRelease.gameObject.SetActive(false);
    }

    public override void OnUse()
    {
        if (playerAmmoSystem.CurrentAmmoCluster <= 0) return;

        AudioManager.Instance.PlaySFX(AudioManager.Instance.audioLibrary.playerClusterShootSfx);
        playerAmmoSystem.DecreaseClusterAmmo(1); //Resta una granada al sistema de munición al usar el lanzagranadas.
        grenadePool.Get();
        shootParticlesPool.Get();
    }

    // ------------- PARTICLES ------------- //

    private ParticlesBehavior OnCreateParticles()
    {
        ParticlesBehavior particlesCopy = Instantiate(shootParticlesPrefab, spawnPoint.position, spawnPoint.rotation);
        particlesCopy.ParticlesPool = shootParticlesPool;
        return particlesCopy;
    }

    private void OnGetParticles(ParticlesBehavior newParticles)
    {
        // Alinear con el spawn del arma
        newParticles.transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
        newParticles.gameObject.SetActive(true);
        newParticles.StartParticlesSystem();
    }

    private void OnReleaseParticles(ParticlesBehavior grenadeParticles)
    {
        grenadeParticles.gameObject.SetActive(false);
    }
}
