using System;
using FSM.Enemy;
using UnityEngine;

public class IdleState : States<EnemyController>
{
    private void OnEnable()
    {
        _controller.Sensor.OnPlayerDetected += PlayerDetected;
    }

    private void PlayerDetected(Transform obj)
    {
        _controller.SetState(_controller.ChaseState);
    }

    public override void OnEnter()
    {
        _controller.Agent.isStopped = true;
        _controller.Agent.ResetPath();
    }

    public override void OnUpdate()
    {

    }

    public override void OnExit()
    {
        _controller.Agent.isStopped = false;
    }

    private void OnDisable()
    {
        _controller.Sensor.OnPlayerDetected -= PlayerDetected;
    }
}
