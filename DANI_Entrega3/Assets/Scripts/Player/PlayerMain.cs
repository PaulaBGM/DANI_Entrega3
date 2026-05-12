using System;
using UnityEngine;

public class PlayerMain : MonoBehaviour
{
    private PlayerHealthSystem playerHealthSystem;
    private PlayerAmmoSystem playerAmmoSystem;
    private PlayerThristSystem playerThirstSystem;

    public event Action<float> OnAmmoGunBoxCollected;
    public event Action<float> OnAmmoGrenadeBoxCollected;
    public event Action<float> OnHealed;

    public event Action<float> OnDehydrated;
    public event Action<float> OnHydrated;

    public event Action OnBriefCaseGetted;
    public event Action OnPlayerDeath; // Evento central de muerte

    private void Awake()
    {
        playerAmmoSystem = GetComponentInChildren<PlayerAmmoSystem>();
        playerHealthSystem = GetComponentInChildren<PlayerHealthSystem>();
        playerThirstSystem = GetComponentInChildren<PlayerThristSystem>();
    }

    private void OnEnable()
    {
        OnAmmoGunBoxCollected += playerAmmoSystem.AddGunAmmo;
        OnAmmoGrenadeBoxCollected += playerAmmoSystem.AddClusterAmmo;
        OnHealed += playerHealthSystem.Heal;
        OnDehydrated += playerHealthSystem.ApplyDamage;
        OnHydrated += playerThirstSystem.Drink;
    }

    private void OnDisable()
    {
        OnAmmoGunBoxCollected -= playerAmmoSystem.AddGunAmmo;
        OnAmmoGrenadeBoxCollected -= playerAmmoSystem.AddClusterAmmo;
        OnHealed -= playerHealthSystem.Heal;
        OnDehydrated -= playerHealthSystem.ApplyDamage;
        OnHydrated -= playerThirstSystem.Drink;
    }

    public void NotifiesDehydrated(float amount) => OnDehydrated?.Invoke(amount);
    public void NotifiesHydrated(float amount) => OnHydrated?.Invoke(amount);
    public void NotifyAmmoGunCollected(float amount) => OnAmmoGunBoxCollected?.Invoke(amount);
    public void NotifyAmmoGrenadeCollected(float amount) => OnAmmoGrenadeBoxCollected?.Invoke(amount);
    public void NotifyHealed(float amount) => OnHealed?.Invoke(amount);

    public void PlayerNotifiesBriefCase()
    {
        OnBriefCaseGetted?.Invoke();
        UIManager.Instance.ShowFinalDialogue();
    }
    
    public void NotifyDeath()
    {
        OnPlayerDeath?.Invoke();
        UIManager.Instance.ShowMissionFailed();
    }
}