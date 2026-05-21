using System;
using UnityEngine;
using System.Collections;

public class PlayerHealthSystem : MonoBehaviour, IDamageable
{
    [SerializeField] private EventManagerSO eventManager;
    [SerializeField] private PlayerMain _playerMain;
    [SerializeField] private float maxHealth = 100f;

    private float currentHealth;

    private void Awake()
    {
        _playerMain = GetComponentInParent<PlayerMain>();
        currentHealth = maxHealth;
    }

    public void ApplyDamage(float damage)
    {
        currentHealth -= damage;
        eventManager.PlayerNotifiesDamaged(currentHealth, maxHealth);

        AudioManager.Instance.PlaySFX(AudioManager.Instance.audioLibrary.shotHitSfx);

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            AudioManager.Instance.PlaySFX(AudioManager.Instance.audioLibrary.playerDeathSfx);

            StartCoroutine(NotifyDeathDelayed());
        }
    }

    private IEnumerator NotifyDeathDelayed()
    {
        yield return new WaitForSeconds(0.5f); // espera 1 frame
        _playerMain.NotifyDeath();
    }

    public void Heal(float healAmount)
    {
        currentHealth = Mathf.Min(currentHealth + healAmount, maxHealth);
        eventManager.InteractableNotifiesHealling(currentHealth, maxHealth);
    }


}