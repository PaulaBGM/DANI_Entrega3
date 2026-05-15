using System;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private EventManagerSO _eventManagerSo;
    private EnemyHealthSystem healthSystem;
    [SerializeField] private Image healthBar;

    private Camera _camera;
    private void Awake()
    {
        healthSystem = GetComponentInParent<EnemyHealthSystem>();
        _camera = Camera.main;   
    }

    private void OnEnable()
    {
        healthSystem.OnHealthChanged += UpdateHealthBar;
    }

    private void UpdateHealthBar(float current, float max)
    {
        if (healthBar != null)
            healthBar.fillAmount = current / max;
    }

    private void LateUpdate()
    {
        // Hacer que el transform mire hacia la cámara
        transform.LookAt(transform.position + _camera.transform.rotation * Vector3.forward, _camera.transform.rotation * Vector3.up);
    }

    private void OnDisable()
    {
        healthSystem.OnHealthChanged -= UpdateHealthBar;
    }
}
