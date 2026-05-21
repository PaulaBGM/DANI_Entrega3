using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class EnemyBullet : MonoBehaviour
{
    public ObjectPool<EnemyBullet> BulletPool { get; set; }

    [Header("Movement")]
    [SerializeField] private float bulletSpeed = 100f;
    [SerializeField] private float bulletLifeTime = 3f;

    [Header("Damage")]
    [SerializeField] private float bulletDamage = 20f;

    public TrailRenderer trailRenderer { get; private set; }

    private Rigidbody rigidbodyBullet;
    private Coroutine lifeCoroutine;

    public bool IsActive { get; private set; } = false;

    private void Awake()
    {
        rigidbodyBullet = GetComponent<Rigidbody>();
        trailRenderer = GetComponent<TrailRenderer>();
    }

    private void OnEnable()
    {
        IsActive = true;

        trailRenderer?.Clear();
        rigidbodyBullet.linearVelocity = Vector3.zero;
        rigidbodyBullet.angularVelocity = Vector3.zero;
    }

    private void OnDisable()
    {
        if (lifeCoroutine != null)
        {
            StopCoroutine(lifeCoroutine);
            lifeCoroutine = null;
        }
    }

    public void Shoot(Vector3 direction)
    {
        if (!IsActive) return;

        // Reiniciamos física y efectos
        rigidbodyBullet.linearVelocity = Vector3.zero;
        rigidbodyBullet.angularVelocity = Vector3.zero;
        trailRenderer?.Clear();

        rigidbodyBullet.AddForce(direction.normalized * bulletSpeed, ForceMode.VelocityChange);

        // Corutina para liberar automáticamente
        lifeCoroutine = StartCoroutine(AutoRelease());
    }

    private IEnumerator AutoRelease()
    {
        yield return new WaitForSeconds(bulletLifeTime);
        ReleasePool();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!IsActive) return;

        GameObject other = collision.gameObject;

        if (other.CompareTag("Player") && other.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.ApplyDamage(bulletDamage);
        }

        // En cualquier caso → se desactiva (incluye suelo, paredes, etc.)
        ReleasePool();
    }
    
    public void ReleasePool()
    {
        if (!IsActive) return;

        IsActive = false;
        BulletPool?.Release(this);
    }
}
