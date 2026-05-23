using UnityEngine;
using System;

public class HearingSystem : MonoBehaviour
{
    [SerializeField]
    private float hearingDistance = 12f;

    [SerializeField]
    private bool debugHearing = true;

    public event Action<Vector3> OnNoiseHeard;

    private float hearingDistanceSqr;

    private void Awake()
    {
        hearingDistanceSqr =
            hearingDistance * hearingDistance;
    }

    private void OnEnable()
    {
        NoiseEmitter.OnNoiseMade += HearNoise;

        Debug.Log(
            $"{name} HEARING ENABLED");
    }

    private void OnDisable()
    {
        NoiseEmitter.OnNoiseMade -= HearNoise;
    }

    private void HearNoise(
        Vector3 noisePosition,
        float noiseRadius)
    {
        Vector3 direction =
            noisePosition - transform.position;

        float distanceSqr =
            direction.sqrMagnitude;

        float totalRange =
            hearingDistance + noiseRadius;

        float totalRangeSqr =
            totalRange * totalRange;

        Debug.Log(
            $"{name} CHECKING NOISE | distance={Mathf.Sqrt(distanceSqr)} | max={totalRange}");

        if (distanceSqr <= totalRangeSqr)
        {
            Debug.Log(
                $"{name} HEARD PLAYER");

            if (debugHearing)
            {
                Debug.DrawLine(
                    transform.position,
                    noisePosition,
                    Color.cyan,
                    2f);
            }

            OnNoiseHeard?.Invoke(
                noisePosition);
        }
        else
        {
            Debug.Log(
                $"{name} TOO FAR TO HEAR");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;

        Gizmos.DrawWireSphere(
            transform.position,
            hearingDistance);
    }
}