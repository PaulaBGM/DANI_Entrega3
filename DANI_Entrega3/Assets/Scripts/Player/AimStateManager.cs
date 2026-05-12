using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;

public class AimStateManager : MonoBehaviour
{
    private PlayerBehavior playerBehavior;

    public Transform camFollowPos;
    public bool robotFound = false;

    [Header("Smooth Settings")]
    [SerializeField] private float smoothSpeed = 10f;

    private float currentXRotation;
    private float currentYRotation;

    [Header("Cameras")]
    [SerializeField] private CinemachineCamera mainCamera;
    [SerializeField] private CinemachineCamera droneCamera;

    [Header("Drone Settings")]
    [SerializeField] private GameObject droneObject;

    private bool isDroneCameraActive = false;

    void Awake()
    {
        playerBehavior = GetComponent<PlayerBehavior>();

        currentXRotation = transform.eulerAngles.y;
        currentYRotation = camFollowPos.localEulerAngles.x;

        // Activar cámara principal al inicio
        if (mainCamera != null)
            mainCamera.Priority = 10;

        if (droneCamera != null)
            droneCamera.Priority = 0;

        // Desactivar drone al inicio
        if (droneObject != null)
        {
            droneObject.SetActive(false);
        }
    }

    void Update()
    {
        if (playerBehavior == null || playerBehavior.IsDead)
            return;

        // Tecla E para cambiar cámara
        if (Keyboard.current.eKey.wasPressedThisFrame && robotFound)
        {
            ToggleCamera();
        }
    }

    private void LateUpdate()
    {
        if (playerBehavior == null || playerBehavior.IsDead)
            return;

        // Solo rotar si NO estamos usando la cámara del drone
        if (!isDroneCameraActive)
        {
            SmoothRotate(camFollowPos);
        }
    }

    public void ShootCamera()
    {
        currentYRotation = Mathf.Clamp(currentYRotation, -11f, 3f);
    }

    public void NormalCamera()
    {
        currentYRotation = Mathf.Clamp(currentYRotation, -15f, 15f);
    }

    private void ToggleCamera()
    {
        isDroneCameraActive = !isDroneCameraActive;

        // Cambiar prioridades de cámaras
        if (mainCamera != null)
            mainCamera.Priority = isDroneCameraActive ? 0 : 10;

        if (droneCamera != null)
            droneCamera.Priority = isDroneCameraActive ? 10 : 0;

        // Activar/desactivar drone
        if (droneObject != null)
        {
            droneObject.SetActive(isDroneCameraActive);
        }

        // Desactivar movimiento del jugador cuando el drone está activo
        PlayerMove playerMove = GetComponent<PlayerMove>();

        if (playerMove != null)
        {
            playerMove.enabled = !isDroneCameraActive;
        }
    }

    private void SmoothRotate(Transform location)
    {
        // Leer movimiento del mouse usando el nuevo Input System
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        float mouseX = mouseDelta.x * smoothSpeed * Time.deltaTime;
        float mouseY = mouseDelta.y * smoothSpeed * Time.deltaTime;

        currentXRotation += mouseX;
        currentYRotation -= mouseY;

        // Limitar rotación vertical
        currentYRotation = Mathf.Clamp(currentYRotation, -15f, 15f);

        // Aplicar rotaciones
        location.localRotation = Quaternion.Euler(currentYRotation, 0f, 0f);
        transform.rotation = Quaternion.Euler(0f, currentXRotation, 0f);
    }
}