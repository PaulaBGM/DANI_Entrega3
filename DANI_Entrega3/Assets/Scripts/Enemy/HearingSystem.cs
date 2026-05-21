using UnityEngine;
using System;

public class HearingSystem : MonoBehaviour
{
    [SerializeField] private float hearingDistance = 12f;

    public event Action<Vector3> OnNoiseHeard;

    private float hearingDistanceSqr;

    private void Awake()
    {
        // Guardamos el cuadrado una sola vez
        hearingDistanceSqr = hearingDistance * hearingDistance;
    }

    private void OnEnable()
    {
        NoiseEmitter.OnNoiseMade += HearNoise;
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

        if (distanceSqr <= totalRangeSqr)
        {
            Debug.Log(
                $"{name} HEARD NOISE AT: {noisePosition}");

            Debug.DrawLine(
                transform.position,
                noisePosition,
                Color.cyan,
                1.5f);

            OnNoiseHeard?.Invoke(
                noisePosition);
        }
    }
}