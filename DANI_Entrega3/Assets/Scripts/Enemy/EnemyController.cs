using System;
using UnityEngine;
using UnityEngine.AI;

namespace FSM.Enemy
{
    public class EnemyController : FSMController<EnemyController>
    {
        // =========================================================
        // SYSTEMS
        // =========================================================

        [Header("Systems")]
        [field: SerializeField] public SensorSystem Sensor { get; set; }
        [field: SerializeField] public HearingSystem Hearing { get; set; }

        [SerializeField] private RagdollSystem _ragdollSystem;
        [SerializeField] private EventManagerSO _eventManagerSO;

        // =========================================================
        // STATS
        // =========================================================

        [Header("Stats")]
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float timeToDestroy = 4.5f;

        // Velocidad máxima usada por estados
        [field: SerializeField] public float MaximumSpeed { get; set; } = 3.5f;

        private EnemyHealthSystem healthSystem;

        // =========================================================
        // STATES
        // =========================================================

        public PatrolState PatrolState { get; set; }
        public ChaseState ChaseState { get; set; }
        public IdleState IdleState { get; set; }

        // Estados de ataque
        public IEnemyAttack CurrentAttackState { get; set; }
        public AttackState MeleeAttackState { get; set; }
        public ShootAttackState ShootAttackState { get; set; }

        // =========================================================
        // COMPONENTS
        // =========================================================

        public NavMeshAgent Agent { get; set; }
        public Animator Animator { get; set; }

        // =========================================================
        // TARGET DATA
        // =========================================================

        public Transform Target { get; set; }

        // Última posición escuchada
        public Vector3 LastHeardPosition { get; set; }

        public bool HasTarget => Target != null;

        // =========================================================
        // INTERNAL
        // =========================================================

        private float currentHealth;
        private bool dead;

        // =========================================================
        // AWAKE
        // =========================================================

        private void Awake()
        {
            Agent = GetComponent<NavMeshAgent>();
            Animator = GetComponent<Animator>();

            _ragdollSystem = GetComponent<RagdollSystem>();
            healthSystem = GetComponent<EnemyHealthSystem>();

            // =========================
            // STATES
            // =========================

            PatrolState = GetComponent<PatrolState>();
            ChaseState = GetComponent<ChaseState>();
            IdleState = GetComponent<IdleState>();

            MeleeAttackState = GetComponent<AttackState>();
            ShootAttackState = GetComponent<ShootAttackState>();

            // =========================
            // INIT STATES
            // =========================

            PatrolState?.InitController(this);
            ChaseState?.InitController(this);
            IdleState?.InitController(this);

            MeleeAttackState?.InitController(this);
            ShootAttackState?.InitController(this);

            // Estado de ataque por defecto
            CurrentAttackState = (IEnemyAttack)ShootAttackState ?? MeleeAttackState;

            currentHealth = maxHealth;

            // Configurar velocidad del NavMeshAgent
            if (Agent != null)
                Agent.speed = MaximumSpeed;

            // =========================
            // INITIAL STATE
            // =========================

            if (PatrolState != null)
                SetState(PatrolState);
            else if (IdleState != null)
                SetState(IdleState);
        }

        // =========================================================
        // ENABLE / DISABLE
        // =========================================================

        private void OnEnable()
        {
            if (healthSystem != null)
                healthSystem.OnEnemyDied += EnemyDead;

            // =========================
            // VISION EVENTS
            // =========================

            if (Sensor != null)
            {
                Sensor.OnPlayerDetected += PlayerDetected;
                Sensor.OnPlayerLost += PlayerLost;
            }

            // =========================
            // HEARING EVENTS
            // =========================

            if (Hearing != null)
            {
                Hearing.OnNoiseHeard += NoiseHeard;
            }
        }

        private void OnDisable()
        {
            if (healthSystem != null)
                healthSystem.OnEnemyDied -= EnemyDead;

            // =========================
            // VISION EVENTS
            // =========================

            if (Sensor != null)
            {
                Sensor.OnPlayerDetected -= PlayerDetected;
                Sensor.OnPlayerLost -= PlayerLost;
            }

            // =========================
            // HEARING EVENTS
            // =========================

            if (Hearing != null)
            {
                Hearing.OnNoiseHeard -= NoiseHeard;
            }
        }

        // =========================================================
        // VISION
        // =========================================================

        private void PlayerDetected(Transform player)
        {
            if (dead) return;

            Target = player;

            if (ChaseState != null)
                SetState(ChaseState);
        }

        private void PlayerLost(Transform player)
        {
            if (dead) return;

            if (Target == player)
            {
                Target = null;

                if (PatrolState != null)
                    SetState(PatrolState);
            }
        }

        // =========================================================
        // HEARING
        // =========================================================

        private void NoiseHeard(Vector3 noisePosition)
        {
            if (dead) return;

            // Si ya tiene target visual ignoramos sonidos
            if (Target != null)
                return;

            LastHeardPosition = noisePosition;

            // Investigar ruido
            if (Agent != null && Agent.enabled)
            {
                Agent.SetDestination(noisePosition);
            }

            if (ChaseState != null)
                SetState(ChaseState);
        }

        // =========================================================
        // ATTACK
        // =========================================================

        public void ChangeToAttackState()
        {
            // Si tiene ataque ranged lo usamos
            if (ShootAttackState != null)
                CurrentAttackState = ShootAttackState;
            else
                CurrentAttackState = MeleeAttackState;

            // Cambiar FSM
            if (CurrentAttackState is States<EnemyController> state)
                SetState(state);
        }

        // =========================================================
        // DEATH
        // =========================================================

        private void EnemyDead()
        {
            if (dead) return;

            dead = true;

            MonoBehaviour[] states =
            {
                PatrolState,
                ChaseState,
                MeleeAttackState,
                ShootAttackState,
                IdleState
            };

            foreach (var state in states)
            {
                if (state != null)
                    state.enabled = false;
            }

            StopAllCoroutines();

            if (Animator != null)
                Animator.enabled = false;

            if (Agent != null)
                Agent.enabled = false;

            enabled = false;

            if (_ragdollSystem != null)
                _ragdollSystem.UpdateBonesState(false);

            Destroy(gameObject, timeToDestroy);
        }
    }
}