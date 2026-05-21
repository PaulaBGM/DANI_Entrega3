using UnityEngine;

public enum WeaponType
{
    None,
    Long,
    Short,
    GrenadeLauncher,
    Knife,
    BackWeapon
}

public class Weapon : MonoBehaviour, IWeapons
{
    public WeaponType weaponType;

    public void DestroyWeapon()
    {
        Destroy(gameObject);
    }
}