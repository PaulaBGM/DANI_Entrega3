using System.Collections;
using System.Collections.Generic;
using FSM.Enemy;
using UnityEngine;

public class PatrolState : States<EnemyController>
{
    [SerializeField] private Transform patrolPath;
    [SerializeField] private float patrolSpeed = 3.5f;
    [SerializeField] private float waitTime = 1f;

    private List<Vector3> patrolPoints = new();
    private int currentIndex;

    private static readonly int Speed = Animator.StringToHash("speed");

    public override void InitController(EnemyController controller)
    {
        base.InitController(controller);

        patrolPoints.Clear();
        foreach (Transform point in patrolPath)
            patrolPoints.Add(point.position);
    }

    private void OnEnable()
    {
        _controller.Sensor.OnPlayerDetected += StartChase;
    }

    private void OnDisable()
    {
        _controller.Sensor.OnPlayerDetected -= StartChase;
        StopAllCoroutines();
    }

    public override void OnEnter()
    {
        _controller.Agent.isStopped = false;
        _controller.Agent.speed = patrolSpeed;
        StartCoroutine(PatrolRoutine());
    }

    public override void OnExit() => StopAllCoroutines();

    public override void OnUpdate()
    {
        _controller.Animator.SetFloat(Speed, _controller.Agent.velocity.magnitude / _controller.MaximumSpeed);
    }

    private IEnumerator PatrolRoutine()
    {
        while (true)
        {
            _controller.Agent.SetDestination(patrolPoints[currentIndex]);

            yield return new WaitUntil(() =>
                !_controller.Agent.pathPending &&
                _controller.Agent.remainingDistance <= _controller.Agent.stoppingDistance);

            yield return new WaitForSeconds(waitTime);
            currentIndex = (currentIndex + 1) % patrolPoints.Count;
        }
    }

    private void StartChase(Transform player)
    {
        StopAllCoroutines();
        _controller.Target = player;
        _controller.SetState(_controller.ChaseState);
    }
}