using UnityEngine;

public class KnifeWeapon : WeaponBase
{
    [SerializeField] private float range = 2f;
    [SerializeField] private LayerMask enemyLayer;

    public override void Fire()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, range, enemyLayer))
        {
            if (hit.collider.TryGetComponent(out IDamageable damageable))
            {
                damageable.ApplyDamage(data.damage);
            }
        }
    }
}