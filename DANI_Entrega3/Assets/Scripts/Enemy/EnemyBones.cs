using System;
using FSM.Enemy;
using UnityEngine;

public class EnemyBones : MonoBehaviour, IDamageable
{
    [SerializeField] private EnemyHealthSystem enemyHealthSystem;
    [SerializeField] private float damageMultiplier;

    private void Awake()
    {
        enemyHealthSystem = GetComponentInParent<EnemyHealthSystem>();
    }
    
    public void ApplyDamage(float damage)
    {
        damage += damageMultiplier;
        enemyHealthSystem.TakeDamage(damage);
    }
}
