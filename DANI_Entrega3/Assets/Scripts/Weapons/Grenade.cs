using UnityEngine;

public class Grenade : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 20f;

    [Header("Explosion")]
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private int maxDamage = 100;

    [Header("Lifetime")]
    [SerializeField] private float lifeTime = 3f;

    [Header("Effects")]
    [SerializeField] private GameObject explosionEffect;

    private Rigidbody rb;
    private bool exploded = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = transform.forward * speed;
        }

        Destroy(gameObject, lifeTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Explode();
    }

    private void Explode()
    {
        if (exploded) return;

        exploded = true;

        if (explosionEffect != null)
        {
            GameObject effect =
                Instantiate(
                    explosionEffect,
                    transform.position,
                    Quaternion.identity
                );

            Destroy(effect, 2f);
        }

        Collider[] colliders =
            Physics.OverlapSphere(
                transform.position,
                explosionRadius
            );

        foreach (Collider nearbyObject in colliders)
        {
            EnemyHealth enemy =
                nearbyObject.GetComponent<EnemyHealth>();

            if (enemy != null)
            {
                float distance =
                    Vector3.Distance(
                        transform.position,
                        nearbyObject.transform.position
                    );

                float damagePercent =
                    1 - (distance / explosionRadius);

                damagePercent = Mathf.Clamp01(damagePercent);

                int damage =
                    Mathf.RoundToInt(maxDamage * damagePercent);

                //enemy.TakeDamage(damage);
            }
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            transform.position,
            explosionRadius
        );
    }
}