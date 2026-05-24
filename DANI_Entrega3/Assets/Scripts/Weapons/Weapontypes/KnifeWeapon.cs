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
            Debug.Log(
                $"KNIFE HIT: {hit.collider.name}");

            Debug.Log(
                $"HIT OBJECT LAYER: {LayerMask.LayerToName(hit.collider.gameObject.layer)}");

            IDamageable damageable =
                hit.collider.GetComponentInParent<IDamageable>();

            if (damageable != null)
            {
                Debug.Log(
                    "IDAMAGEABLE FOUND");

                damageable.ApplyDamage(
                    data.damage);

                Debug.Log(
                    $"KNIFE DAMAGE APPLIED: {data.damage}");
            }
            else
            {
                Debug.LogError(
                    $"NO IDAMAGEABLE FOUND ON: {hit.collider.name}");
            }
        }
        else
        {
            Debug.LogError(
                "KNIFE RAYCAST MISSED");
        }
    }
}