using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/Weapon Data")]
public class WeaponData : ScriptableObject
{
    public WeaponType weaponType;

    public float damage;
    public float fireRate;

    public bool automatic;

    public int ammoCost;

    public float noiseRadius;

    public GameObject projectilePrefab;
}