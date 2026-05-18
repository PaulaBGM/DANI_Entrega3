using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    public WeaponData data;
    public WeaponType weaponType;

    public abstract void Fire();
}