using System;
using UnityEngine;

public class EnemyHealthSystem : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private RagdollSystem ragdollSystem;

    private float currentHealth;
    private bool isDead;

    public event Action<float, float> OnHealthChanged; // Para la barra de vida
    
    public event Action OnEnemyDied; // Opcional para que EnemyController o efectos lo escuchen

    private void Awake()
    {
        currentHealth = maxHealth;
        if (ragdollSystem == null)
            ragdollSystem = GetComponent<RagdollSystem>();
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        AudioManager.Instance.PlaySFX(AudioManager.Instance.audioLibrary.enemyDeathSfx);
        
        // Notifica que murió
        OnEnemyDied?.Invoke();
    }

    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
}