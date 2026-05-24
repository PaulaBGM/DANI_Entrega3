using UnityEngine;

public class KnifeWeapon : WeaponBase
{
    [SerializeField]
    private float range = 2f;

    [SerializeField]
    private LayerMask enemyLayer;

    public override void Fire()
    {
        Ray ray =
            new Ray(
                Camera.main.transform.position,
                Camera.main.transform.forward);

        if (Physics.Raycast(
            ray,
            out RaycastHit hit,
            range,
            enemyLayer))
        {
            Debug.Log(
                $"KNIFE HIT: {hit.collider.name}");

            IDamageable damageable =
                hit.collider.GetComponentInParent<IDamageable>();

            if (damageable != null)
            {
                damageable.ApplyDamage(
                    data.damage);

                Debug.Log(
                    $"KNIFE DAMAGE: {data.damage}");
            }
        }
    }
}