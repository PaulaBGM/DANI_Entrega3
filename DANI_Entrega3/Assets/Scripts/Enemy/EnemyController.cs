using System;
using UnityEngine;
using UnityEngine.AI;

namespace FSM.Enemy
{
    public class EnemyController : FSMController<EnemyController>
    {
        [field: SerializeField] public SensorSystem Sensor { get; set; }
        [SerializeField] private RagdollSystem _ragdollSystem;
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float timeToDestroy = 4.5f;
        [SerializeField] private EventManagerSO _eventManagerSO;

        private EnemyHealthSystem healthSystem;

        public PatrolState PatrolState { get; set; }
        public Transform Target { get; set; }
        public Animator Animator { get; set; }
        public float MaximumSpeed { get; set; }
        public ChaseState ChaseState { get; set; }
        public IdleState IdleState { get; set; }


        // Estados de ataque implementando IEnemyAttack
        public IEnemyAttack CurrentAttackState { get; set; }
        public AttackState MeleeAttackState { get; set; }
        public ShootAttackState ShootAttackState { get; set; }

        public NavMeshAgent Agent { get; set; }
        private float currentHealth;
        private bool dead;
        
        private void Awake()
        {
            Agent = GetComponent<NavMeshAgent>();
            Animator = GetComponent<Animator>();
            _ragdollSystem = GetComponent<RagdollSystem>();

            PatrolState = GetComponent<PatrolState>();
            ChaseState = GetComponent<ChaseState>();
            MeleeAttackState = GetComponent<AttackState>();
            ShootAttackState = GetComponent<ShootAttackState>();
            IdleState = GetComponent<IdleState>();
            
            healthSystem = GetComponent<EnemyHealthSystem>();
            
            // Inicializar los estados con el controlador
            PatrolState?.InitController(this);
            ChaseState?.InitController(this);
            MeleeAttackState?.InitController(this);
            ShootAttackState?.InitController(this);
            IdleState?.InitController(this);

            // Inicializar CurrentAttackState (por defecto melee o ranged según componentes)
            CurrentAttackState = (IEnemyAttack)ShootAttackState ?? MeleeAttackState;

            currentHealth = maxHealth;
            
            // Elegir estado inicial disponible
            if (PatrolState != null)
                SetState(PatrolState);
            else if (IdleState != null)
                SetState(IdleState);
        }

        private void OnEnable()
        {
            healthSystem.OnEnemyDied += EnemyDead;
        }

        // Cambiar ataque dinámicamente
        public void ChangeToAttackState()
        {
            // Por ejemplo, si tiene ShootAttackState disponible, usamos ranged
            if (ShootAttackState != null)
                CurrentAttackState = ShootAttackState;
            else
                CurrentAttackState = MeleeAttackState;

            // Cambiamos el estado de la FSM si implementa States<T>
            if (CurrentAttackState is States<EnemyController> state)
                SetState(state);
        }

        private void EnemyDead()
        {
            MonoBehaviour[] states = { PatrolState, ChaseState, MeleeAttackState, ShootAttackState };
            foreach (var state in states)
                if (state != null) state.enabled = false;
            
            StopAllCoroutines();

            Animator.enabled = false;
            Agent.enabled = false;
            this.enabled = false;

            _ragdollSystem.UpdateBonesState(false);

            Destroy(gameObject, timeToDestroy);
        }

        private void OnDisable()
        {
            healthSystem.OnEnemyDied -= EnemyDead;
        }
    }
}
