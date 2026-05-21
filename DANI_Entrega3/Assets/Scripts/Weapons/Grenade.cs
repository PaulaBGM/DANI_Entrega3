using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class Grenade : MonoBehaviour
{
    public ObjectPool<Grenade> GrenadePool {get; set;}

    private ObjectPool<ParticlesBehavior> grenadeParticlesPool;
    
    [SerializeField] private ParticlesBehavior grenadeParticlesPrefab;

    [Header("Move/Time")]
    [SerializeField] private float impulseForce = 30f;
    [SerializeField] private float timeToRelease = 5f;
    
    [Header("Damage")]
    [SerializeField] private float damageRadius = 4f;
    [SerializeField] private float damageGenerated = 25f;
    [SerializeField] private LayerMask damageLayerMask;
    
    private Rigidbody _rigidbody;

    private void Awake()
    {
       grenadeParticlesPool = new ObjectPool<ParticlesBehavior>(OnCreateParticles, OnGetParticles, OnReleaseParticles);
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void Launch(Vector3 direction)
    {
        _rigidbody.linearVelocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
        _rigidbody.AddForce(direction.normalized * impulseForce, ForceMode.Impulse);
        StartCoroutine(WaitAndRelease());
    }

    private IEnumerator WaitAndRelease()
    {
        yield return new WaitForSeconds(timeToRelease);
        Explode();
        GrenadePool.Release(this);
    }

    private void Explode()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, damageRadius, damageLayerMask);
        foreach (Collider hit in hits)
        {
            IDamageable damageable = hit.GetComponentInParent<IDamageable>();
            if (damageable != null)
            {
                float distance = Vector3.Distance(transform.position, hit.transform.position);
                float damage = Mathf.Clamp01(1 - (distance / damageRadius)) * damageGenerated;
                damageable.ApplyDamage(damage);
            }
        }

        AudioManager.Instance.PlaySFX(AudioManager.Instance.audioLibrary.grenadeSfx);
        
        //Hacer aparecer las partículas en la posición y rotación de la granada
        var explosion = grenadeParticlesPool.Get();
        explosion.transform.position = transform.position;
        explosion.transform.rotation = Quaternion.identity;
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, damageRadius);
    }
    
    //--------------------PARTICLES----------------------//
    private ParticlesBehavior OnCreateParticles()
    {
        ParticlesBehavior particlesCopy = Instantiate(grenadeParticlesPrefab);
        particlesCopy.ParticlesPool = grenadeParticlesPool;
        return particlesCopy;
    }

    private void OnGetParticles(ParticlesBehavior newParticles)
    {
        newParticles.gameObject.SetActive(true);
        newParticles.StartParticlesSystem();
    }

    private void OnReleaseParticles(ParticlesBehavior grenadeParticles)
    {
        grenadeParticles.gameObject.SetActive(false);
    }
}
