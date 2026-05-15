using System;
using UnityEngine;

[CreateAssetMenu(fileName = "EventManager", menuName = "Scriptable Objects/EventManagerSO")]
public class EventManagerSO : ScriptableObject
{
    public event Action<float, float> OnPlayerDamaged;
    public event Action<float, float> OnPlayerHealed;
    public event Action<float, float> OnPlayerThirstChanged;
    public event Action<float, float> OnPlayerUseAmmoGun;
    public event Action<float, float> OnPlayerUseGrenade;
    public event Action<float, float> OnAmmoGunGetted;
    public event Action<float, float> OnAmmoGrenadeGetted;
    public event Action<GameObject, float, float> OnEnemyDamaged;    
    public void PlayerNotifiesDamaged(float currentHealth, float maxHealth) => OnPlayerDamaged?.Invoke(currentHealth, maxHealth);

    public void PlayerNotifiesThirstChanged(float current, float max)  => OnPlayerThirstChanged?.Invoke(current, max);
    
    public void PlayerUseGunAmmo(float currentAmmoGun, float maxAmmoGun)  => OnPlayerUseAmmoGun?.Invoke(currentAmmoGun, maxAmmoGun);

    public void PlayerUseGrenadeAmmo(float currentAmmoGrenade, float maxAmmoGrenade)  => OnPlayerUseGrenade?.Invoke(currentAmmoGrenade, maxAmmoGrenade);
    
    public void InteractableNotifiesHealling(float currentHealth, float maxHealth)  => OnPlayerHealed?.Invoke(currentHealth, maxHealth);
    
    public void AmmoGunGetted(float currentAmmo, float maxAmmo)  => OnAmmoGunGetted?.Invoke(currentAmmo, maxAmmo);

    public void AmmoGrenadeGetted(float currentAmmo, float maxAmmo)  => OnAmmoGrenadeGetted?.Invoke(currentAmmo, maxAmmo);

    public void EnemyNotifiesDamaged(GameObject enemy, float currentHealth, float maxHealth)  => OnEnemyDamaged?.Invoke(enemy, currentHealth, maxHealth);
}

