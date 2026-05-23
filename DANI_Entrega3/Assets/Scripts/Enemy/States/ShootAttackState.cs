using DG.Tweening;
using FSM.Enemy;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class ShootAttackState : States<EnemyController>, IEnemyAttack
{
    private static readonly int Attacking = Animator.StringToHash("attacking");

    [Header("Attack Settings")]
    [SerializeField] private float attackDistance = 10f;
    [SerializeField] private float smoothGaze = 2f;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject gunGameObject;
    [SerializeField] private EnemyBullet bulletPrefab;
    [SerializeField] private float offSet = 10f;
    
    private ObjectPool<EnemyBullet> bulletPool;

    private void OnEnable()
    {
        _controller.Sensor.OnPlayerLost += StopShooting;
    }

    public override void OnEnter()
    {
        gunGameObject.SetActive(true);
        _controller.Agent.isStopped = true;
        _controller.Animator.SetBool(Attacking, true);

        bulletPool ??= new ObjectPool<EnemyBullet>(OnCreateBullet, OnGetBullet, OnReleaseBullet);
    }

    public void OnAttack(Transform target)
    {
        
    }
    
    public override void OnUpdate()
    {
        if (_controller.Target == null)
        {
            StopShooting(null);
            return;
        }

        float distance =
            Vector3.Distance(_controller.transform.position, _controller.Target.position);

        if (distance > attackDistance)
        {
            StopShooting(_controller.Target);
            return;
        }

        Vector3 lookTarget =
            _controller.Target.position + Vector3.right * offSet;

        transform.DOLookAt(lookTarget, smoothGaze, AxisConstraint.Y);
    }

    private EnemyBullet OnCreateBullet()
    {
        EnemyBullet bullet = Instantiate(bulletPrefab);
        bullet.BulletPool = bulletPool;
        return bullet;
    }

    private void OnGetBullet(EnemyBullet bullet)
    {
        bullet.gameObject.SetActive(true);
        bullet.transform.position = firePoint.position;
        bullet.transform.rotation = firePoint.rotation;

        // Reiniciamos física y trail por si acaso
        var rb = bullet.GetComponent<Rigidbody>();
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        bullet.trailRenderer?.Clear();
    }

    private void OnReleaseBullet(EnemyBullet bullet)
    {
        var rb = bullet.GetComponent<Rigidbody>();
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.Sleep();

        bullet.gameObject.SetActive(false);
    }
    
    private void StopShooting(Transform player)
    {
        if (_controller == null)
            return;

        _controller.Animator.SetBool(Attacking, false);

        if (gunGameObject != null)
            gunGameObject.SetActive(false);

        _controller.SetState(_controller.ChaseState);
    }
    
    private void OnDisable()
    {
        _controller.Sensor.OnPlayerLost -= StopShooting;
    }

    // Método que dispara la bala
    private void OnShootAttack()
    {
        if (_controller == null || _controller.Target == null || firePoint == null)
            return;

        if (bulletPool == null)
            return;

        var bullet = bulletPool.Get();

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(AudioManager.Instance.audioLibrary.enemyShootSfx);

        Vector3 targetDirection =
            (_controller.Target.position - firePoint.position).normalized;

        if (!bullet.IsActive)
            return;

        bullet.Shoot(targetDirection);
    }

    private void ShootAttackFinished()
    {
        if (_controller == null || _controller.Target == null)
            return;

        float distance =
            Vector3.Distance(_controller.transform.position,
                _controller.Target.position);

        if (distance > attackDistance)
        {
            gunGameObject.SetActive(false);
            _controller.SetState(_controller.ChaseState);
        }
    }

    public override void OnExit()
    {
        _controller.Animator.SetBool(Attacking, false);
        gunGameObject.SetActive(false);
    }
    
}
