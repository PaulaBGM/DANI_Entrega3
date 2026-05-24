using FSM.Enemy;
using DG.Tweening;
using UnityEngine;

public class AttackState : States<EnemyController>, IEnemyAttack
{
    private static readonly int Attacking = Animator.StringToHash("attacking");

    [SerializeField] private float attackDistance = 2.5f;
    [SerializeField] private float smoothGaze = 2f;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRadius = 1f;
    [SerializeField] private int damage = 15;

    private Tween lookTween;

    public override void OnEnter()
    {
        _controller.Agent.isStopped = true;
        _controller.Animator.SetBool(Attacking, true);

        if (_controller.Target != null)
        {
            lookTween = transform.DOLookAt(
                _controller.Target.position,
                smoothGaze,
                AxisConstraint.Y
            );
        }
    }

    public override void OnUpdate()
    {
        // Si pierde el target durante el ataque
        if (_controller.Target == null)
        {
            ExitAttack();
            return;
        }

        // Si el target se aleja demasiado
        if (Vector3.Distance(transform.position, _controller.Target.position) > attackDistance)
        {
            ExitAttack();
        }
    }

    // Se llama mediante Animation Event
    public void OnAttack(Transform target)
    {
        if (attackPoint == null)
        {
            Debug.LogError("Attack Point no asignado en AttackState");
            return;
        }

        Collider[] colliders = Physics.OverlapSphere(
            attackPoint.position,
            attackRadius
        );

        foreach (Collider coll in colliders)
        {
            if (coll.TryGetComponent(out PlayerHealthSystem playerHealth))
            {
                playerHealth.ApplyDamage(damage);

                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlaySFX(
                        AudioManager.Instance.audioLibrary.punchSfx
                    );
                }
            }
        }
    }

    // Se llama mediante Animation Event
    private void AttackFinished()
    {
        if (_controller.Target == null)
        {
            ExitAttack();
            return;
        }

        if (Vector3.Distance(transform.position, _controller.Target.position) > attackDistance)
        {
            ExitAttack();
        }
    }

    private void ExitAttack()
    {
        _controller.Animator.SetBool(Attacking, false);
        _controller.Agent.isStopped = false;

        if (_controller.ChaseState != null)
        {
            _controller.SetState(_controller.ChaseState);
        }
    }

    public override void OnExit()
    {
        if (lookTween != null && lookTween.IsActive())
        {
            lookTween.Kill();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}