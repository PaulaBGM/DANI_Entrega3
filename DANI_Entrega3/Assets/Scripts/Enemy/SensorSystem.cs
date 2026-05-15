using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class SensorSystem : MonoBehaviour
{
    [SerializeField] private float sensorDistance = 9f;
    [SerializeField] private float sensorAngle = 65f;
    [SerializeField] private LayerMask obstaclesMask;
    
    public event Action<Transform> OnPlayerDetected, OnPlayerLost;
    
    [SerializeField] private Transform headTransform;       // referencia al hueso de la cabeza

    private SphereCollider sensorCollider;

    private void Awake()
    {
        sensorCollider = GetComponent<SphereCollider>();
        sensorCollider.radius = sensorDistance;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CheckDetection(other);
        }
    }

    private void CheckDetection(Collider other)
    {
        Vector3 directionToTarget = (other.transform.position - transform.position).normalized;
        float distanceToTarget = Vector3.Distance(headTransform.position, other.transform.position);

        //Si hay un obstáculo en medio no es valida la detección.
        if(Physics.Raycast(headTransform.position, directionToTarget, distanceToTarget, obstaclesMask)) return;
        
        //Revisa el ángulo de visión
        float angleToTarget = Vector3.Angle(headTransform.forward, directionToTarget);

        if (angleToTarget < sensorAngle / 2f && distanceToTarget <= sensorDistance)
        {
            OnPlayerDetected?.Invoke(other.transform);
        }
        else  OnPlayerLost?.Invoke(other.transform);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnPlayerLost?.Invoke(other.transform);
        }
    }
    
    // Dibujo de rayos para depuración visual
    private void OnDrawGizmosSelected()
    {
        if (headTransform == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(headTransform.position, sensorDistance);

        // Dibuja el cono de visión de la cabeza
        Vector3 forward = headTransform.forward * sensorDistance;
        Quaternion leftRayRotation = Quaternion.AngleAxis(-sensorAngle / 2, Vector3.up);
        Quaternion rightRayRotation = Quaternion.AngleAxis(sensorAngle / 2, Vector3.up);
        Vector3 leftRayDirection = leftRayRotation * forward;
        Vector3 rightRayDirection = rightRayRotation * forward;

        Gizmos.color = Color.red;
        Gizmos.DrawRay(headTransform.position, leftRayDirection);
        Gizmos.DrawRay(headTransform.position, rightRayDirection);
    }
}
