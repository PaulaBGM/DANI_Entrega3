using UnityEngine;

public class KnifeWeapon : WeaponBase
{
    [SerializeField]
    private float range = 2f;

    [SerializeField]
    private LayerMask enemyLayer;

    public override void Fire()
    {
        Debug.Log("KNIFE FIRE");

        if (Camera.main == null)
        {
            Debug.LogError("MAIN CAMERA NULL");
            return;
        }

        Ray ray =
            new Ray(
                Camera.main.transform.position,
                Camera.main.transform.forward);

        Debug.DrawRay(
            ray.origin,
            ray.direction * range,
            Color.red,
            2f);

        Debug.Log(
            $"KNIFE RAYCAST | origin={ray.origin} | direction={ray.direction}");

        if (Physics.Raycast(
            ray,
            out RaycastHit hit,
            range,
            enemyLayer))
        {
           

            IDamageable damageable =
                hit.collider.GetComponentInParent<IDamageable>();

            if (damageable != null)
            {

                damageable.ApplyDamage(
                    data.damage);
            }

        }

    }
}