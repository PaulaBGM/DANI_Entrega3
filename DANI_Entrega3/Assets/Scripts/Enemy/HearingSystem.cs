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

    private void HearNoise(Vector3 noisePosition, float noiseRadius)
    {
        Vector3 direction = noisePosition - transform.position;

        // Distancia al cuadrado
        float distanceSqr = direction.sqrMagnitude;

        // Radio total al cuadrado
        float totalRange = hearingDistance + noiseRadius;
        float totalRangeSqr = totalRange * totalRange;

        // Comparación SIN sqrt
        if (distanceSqr <= totalRangeSqr)
        {
            OnNoiseHeard?.Invoke(noisePosition);
        }
    }
}