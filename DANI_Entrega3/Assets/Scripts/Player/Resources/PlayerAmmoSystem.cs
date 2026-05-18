using UnityEngine;

public class PlayerAmmoSystem : MonoBehaviour
{
    [SerializeField] private EventManagerSO eventManager;

    [Header("Ammo Settings")]
    [SerializeField] private float maxAmmoGun = 50f;
    [SerializeField] private float maxAmmoCluster = 5f;

    public float CurrentAmmoGun { get; private set; }
    public float CurrentAmmoCluster { get; private set; }

    private void Awake()
    {
        CurrentAmmoGun = maxAmmoGun;
        CurrentAmmoCluster = 0;
    }

    private void Start()
    {
        // Actualizar UI al inicio
        eventManager.PlayerUseGunAmmo(CurrentAmmoGun, maxAmmoGun);
        eventManager.PlayerUseGrenadeAmmo(CurrentAmmoCluster, maxAmmoCluster);
    }

    public void AddGunAmmo(float amount)
    {
        CurrentAmmoGun = Mathf.Min(CurrentAmmoGun + amount, maxAmmoGun);
        eventManager.AmmoGunGetted(CurrentAmmoGun, maxAmmoGun);
    }

    public void AddClusterAmmo(float amount)
    {
        CurrentAmmoCluster = Mathf.Min(CurrentAmmoCluster + amount, maxAmmoCluster);
        eventManager.AmmoGrenadeGetted(CurrentAmmoCluster, maxAmmoCluster);
    }

    public void DecreaseGunAmmo(float amount)
    {
        CurrentAmmoGun = Mathf.Max(CurrentAmmoGun - amount, 0);
        eventManager.PlayerUseGunAmmo(CurrentAmmoGun, maxAmmoGun);
    }

    public void DecreaseClusterAmmo(float amount)
    {
        CurrentAmmoCluster = Mathf.Max(CurrentAmmoCluster - amount, 0);
        eventManager.PlayerUseGrenadeAmmo(CurrentAmmoCluster, maxAmmoCluster);
    }
}