using UnityEngine;

public class GrenadeLauncherWeapon : WeaponBase
{
    [SerializeField] private ClusterBehavior clusterBehavior;

    public override void Fire()
    {
        clusterBehavior.OnUse();
    }
}