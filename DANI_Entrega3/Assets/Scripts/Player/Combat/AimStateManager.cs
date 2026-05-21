using UnityEngine;

public class AimStateManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] public Transform camFollowPos;

    [SerializeField] private Camera mainCamera;

    [SerializeField] private Transform playerModel;

    [Header("Settings")]
    [SerializeField] private float sensitivity = 120f;

    [SerializeField] private float aimSensitivityMultiplier = 0.6f;

    [SerializeField] private float smoothSpeed = 10f;

    [SerializeField] private float aimRotateSpeed = 15f;

    [Header("Vertical Clamp")]
    [SerializeField] private float normalMinPitch = -15f;

    [SerializeField] private float normalMaxPitch = 15f;

    [SerializeField] private float aimMinPitch = -11f;

    [SerializeField] private float aimMaxPitch = 3f;

    [Header("Aim FOV")]
    [SerializeField] private float normalFov = 60f;

    [SerializeField] private float aimFov = 40f;

    [SerializeField] private float fovSpeed = 10f;

    [Header("Camera Positions")]
    [SerializeField] private Vector3 normalCameraPosition;

    [SerializeField]
    private Vector3 aimCameraPosition =
        new Vector3(0.5f, 1.5f, -1.2f);

    [SerializeField] private float cameraMoveSpeed = 10f;

    [Header("Crosshair")]
    [SerializeField] private GameObject crosshair;

    [Header("Combat")]
    [SerializeField] private bool rotatePlayerWhileAiming = true;

    private PlayerInputHandler input;

    private float currentXRotation;

    private float currentYRotation;

    private float minPitch;

    private float maxPitch;

    private bool isAiming;

    public bool IsAiming => isAiming;

    private void Awake()
    {
        input = GetComponent<PlayerInputHandler>();

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        currentXRotation =
            transform.eulerAngles.y;

        currentYRotation =
            camFollowPos.localEulerAngles.x;

        normalCameraPosition =
            camFollowPos.localPosition;

        Cursor.lockState =
            CursorLockMode.Locked;

        Cursor.visible = false;

        NormalCamera();
    }

    private void LateUpdate()
    {
        HandleAimState();

        RotateCamera();

        HandleAimCamera();

        HandleCameraPosition();

        HandleCrosshair();

        RotatePlayerToCamera();
    }

    // =========================
    // AIM STATE
    // =========================

    private void HandleAimState()
    {
        isAiming =
            input.AimPressed;
    }

    // =========================
    // CAMERA ROTATION
    // =========================

    private void RotateCamera()
    {
        Vector2 look =
            input.LookInput;

        float currentSensitivity =
            isAiming
            ? sensitivity * aimSensitivityMultiplier
            : sensitivity;

        float mouseX =
            look.x *
            currentSensitivity *
            Time.deltaTime;

        float mouseY =
            look.y *
            currentSensitivity *
            Time.deltaTime;

        currentXRotation += mouseX;

        currentYRotation -= mouseY;

        currentYRotation =
            Mathf.Clamp(
                currentYRotation,
                minPitch,
                maxPitch);

        Quaternion targetBodyRotation =
            Quaternion.Euler(
                0f,
                currentXRotation,
                0f);

        Quaternion targetCameraRotation =
            Quaternion.Euler(
                currentYRotation,
                0f,
                0f);

        transform.rotation =
            Quaternion.Slerp(
                transform.rotation,
                targetBodyRotation,
                smoothSpeed * Time.deltaTime);

        camFollowPos.localRotation =
            Quaternion.Slerp(
                camFollowPos.localRotation,
                targetCameraRotation,
                smoothSpeed * Time.deltaTime);
    }

    // =========================
    // CAMERA POSITION
    // =========================

    private void HandleCameraPosition()
    {
        Vector3 targetPosition =
            isAiming
            ? aimCameraPosition
            : normalCameraPosition;

        camFollowPos.localPosition =
            Vector3.Lerp(
                camFollowPos.localPosition,
                targetPosition,
                cameraMoveSpeed * Time.deltaTime);
    }

    // =========================
    // AIM CAMERA
    // =========================

    private void HandleAimCamera()
    {
        if (isAiming)
        {
            ShootCamera();
        }
        else
        {
            NormalCamera();
        }

        float targetFov =
            isAiming
            ? aimFov
            : normalFov;

        mainCamera.fieldOfView =
            Mathf.Lerp(
                mainCamera.fieldOfView,
                targetFov,
                fovSpeed * Time.deltaTime);
    }

    // =========================
    // CROSSHAIR
    // =========================

    private void HandleCrosshair()
    {
        if (crosshair == null)
            return;

        crosshair.SetActive(isAiming);
    }

    // =========================
    // PLAYER ROTATION
    // =========================

    private void RotatePlayerToCamera()
    {
        if (!rotatePlayerWhileAiming)
            return;

        if (!isAiming)
            return;

        Vector3 lookDirection =
            mainCamera.transform.forward;

        lookDirection.y = 0f;

        if (lookDirection.sqrMagnitude <= 0.01f)
            return;

        Quaternion targetRotation =
            Quaternion.LookRotation(
                lookDirection);

        playerModel.rotation =
            Quaternion.Slerp(
                playerModel.rotation,
                targetRotation,
                aimRotateSpeed * Time.deltaTime);
    }

    // =========================
    // AIM MODE
    // =========================

    public void ShootCamera()
    {
        minPitch = aimMinPitch;

        maxPitch = aimMaxPitch;
    }

    // =========================
    // NORMAL MODE
    // =========================

    public void NormalCamera()
    {
        minPitch = normalMinPitch;

        maxPitch = normalMaxPitch;
    }
}