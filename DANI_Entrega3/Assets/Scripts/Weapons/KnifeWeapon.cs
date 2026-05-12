using UnityEngine;

public class KnifeWeapon : Weapon
{
    [SerializeField] private float knifeDistance = 2f;
    [SerializeField] private float knifeDamage = 30f;

    public override void OnUse()
    {
        if (Physics.Raycast(
            mainCamera.transform.position,
            mainCamera.transform.forward,
            out RaycastHit hit,
            knifeDistance))
        {
            if (hit.transform.TryGetComponent(
                out EnemyBones enemy))
            {
                enemy.ApplyDamage(knifeDamage);
            }
        }
    }
}