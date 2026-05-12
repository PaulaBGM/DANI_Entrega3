using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
    Knife,
    Pistol,
    Rifle,
    GrenadeLauncher
}

public abstract class Weapon : MonoBehaviour
{
    [Header("Info")]
    public WeaponType weaponType;

    [Header("Settings")]
    public float fireRate = 0.2f;

    [Header("References")]
    public Transform shootPoint;

    protected PlayerAmmoSystem ammoSystem;
    protected Camera mainCamera;

    protected virtual void Awake()
    {
        ammoSystem = GetComponentInParent<PlayerAmmoSystem>();
        mainCamera = Camera.main;
    }

    public abstract void OnUse();

    public virtual bool HasAmmo()
    {
        return true;
    }
}