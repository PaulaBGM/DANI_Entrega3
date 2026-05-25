using System;
using UnityEngine;

public class SensorSystem : MonoBehaviour
{
    [SerializeField] private float sensorDistance = 9f;
    [SerializeField] private float sensorAngle = 65f;
    [SerializeField] private LayerMask obstaclesMask;
    [SerializeField] private Transform headTransform;

    public event Action<Transform> OnPlayerDetected;
    public event Action<Transform> OnPlayerLost;

    private Transform currentTarget;
    private bool hasLOS;

    private void Awake()
    {
        var col = GetComponent<SphereCollider>();
        col.radius = sensorDistance;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        CheckVision(other.transform);
    }

    private void CheckVision(Transform target)
    {
        Vector3 dir = (target.position - headTransform.position).normalized;
        float distance = Vector3.Distance(headTransform.position, target.position);

        if (distance > sensorDistance)
            return;

        float angle = Vector3.Angle(headTransform.forward, dir);
        if (angle > sensorAngle * 0.5f)
            return;

        bool blocked = Physics.Raycast(
            headTransform.position,
            dir,
            distance,
            obstaclesMask
        );

        if (blocked)
        {
            SetLost(target);
            return;
        }

        SetDetected(target);
    }

    private void SetDetected(Transform target)
    {
        if (currentTarget != target)
        {
            currentTarget = target;
            OnPlayerDetected?.Invoke(target);
        }

        hasLOS = true;
    }

    private void SetLost(Transform target)
    {
        if (!hasLOS) return;

        hasLOS = false;
        currentTarget = null;

        OnPlayerLost?.Invoke(target);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (currentTarget == other.transform)
        {
            SetLost(other.transform);
        }
    }
}