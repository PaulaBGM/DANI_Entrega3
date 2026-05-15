using System;
using FSM.Enemy;
using UnityEngine;
using UnityEngine.AI;

public class ChaseState : States<EnemyController>
{
    [SerializeField] private float chaseSpeed = 4f;
    [SerializeField] private float chaseStopDistance = 2.5f;
    
    private static readonly int Speed = Animator.StringToHash("speed");
    public override void InitController(EnemyController controller)
    {
        base.InitController(controller);
        
        _controller.MaximumSpeed =chaseSpeed; //Velocidad máxima de todo el sistema.
    }

    public override void OnEnter()
    {
        if (_controller.Agent == null || !_controller.Agent.isOnNavMesh)
            return;
        
        _controller.Agent.isStopped = false;
        _controller.Agent.stoppingDistance = chaseStopDistance;
        _controller.Agent.speed = chaseSpeed;
    }

    private void OnEnable()
    {
        _controller.Sensor.OnPlayerDetected += GetTarget;
    }

    private void GetTarget (Transform newTarget)
    {
        _controller.Target = newTarget;
    }

    public override void OnUpdate()
    {
        if (_controller.Agent == null || !_controller.Agent.isOnNavMesh || _controller.Target == null)
            return;
        
        _controller.Animator.SetFloat(Speed, _controller.Agent.velocity.magnitude/_controller.MaximumSpeed);
        _controller.Agent.SetDestination(_controller.Target.position);

        if (Vector3.Distance(transform.position, _controller.Target.position) > chaseStopDistance)
        {
            _controller.Agent.SetDestination(_controller.Target.position);
        }
        else
        {
            // Está dentro del rango de ataque → pasamos al ataque correcto
            _controller.Agent.isStopped = true;
            _controller.ChangeToAttackState();
        }
    }

    public override void OnExit()
    {
        _controller.Sensor.OnPlayerDetected -= GetTarget;
    }

    private void OnDisable()
    {
        _controller.Sensor.OnPlayerDetected -= GetTarget;
    }
}
