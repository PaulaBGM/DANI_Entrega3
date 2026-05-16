using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class ParticlesBehavior : MonoBehaviour
{
    public ObjectPool<ParticlesBehavior> ParticlesPool {get; set;}

    private ParticleSystem particleSystemInstance;

    private void Awake()
    {
        particleSystemInstance = GetComponent<ParticleSystem>();
    }

    public void StartParticlesSystem()
    {
        particleSystemInstance.Play();

        // Retornar al pool cuando las partículas terminen
        StartCoroutine(ReleaseParticlesSystem());
    }

    private IEnumerator ReleaseParticlesSystem()
    {
        // Esperar hasta que el sistema de partículas deje de estar vivo
        yield return new WaitWhile(() => particleSystemInstance.IsAlive(true));

        transform.SetParent(null);

        // Desactivar y devolver al pool
        gameObject.SetActive(false);
        ParticlesPool.Release(this);
    }
}
