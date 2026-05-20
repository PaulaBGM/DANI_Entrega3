using UnityEngine;

public class AimStateManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] public Transform camFollowPos;

    [SerializeField] private Camera mainCamera;

    [Header("Settings")]
    [SerializeField] private float sensitivity = 120f;

    [SerializeField] private float smoothSpeed = 10f;

    [Header("Vertical Clamp")]
    [SerializeField] private float normalMinPitch = -15f;

    [SerializeField] private float normalMaxPitch = 15f;

    [SerializeField] private float aimMinPitch = -11f;

    [SerializeField] private float aimMaxPitch = 3f;

    [Header("Aim FOV")]
    [SerializeField] private float normalFov = 60f;

    [SerializeField] private float aimFov = 40f;

    [SerializeField] private float fovSpeed = 10f;

    private PlayerInputHandler input;

    private float currentXRotation;

    private float currentYRotation;

    private float minPitch;

    private float maxPitch;

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

        Cursor.lockState =
            CursorLockMode.Locked;

        Cursor.visible = false;

        NormalCamera();
    }

    private void LateUpdate()
    {
        RotateCamera();

        HandleAimCamera();
    }

    // =========================
    // ROTATION
    // =========================

    private void RotateCamera()
    {
        Vector2 look =
            input.LookInput;

        float mouseX =
            look.x *
            sensitivity *
            Time.deltaTime;

        float mouseY =
            look.y *
            sensitivity *
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
    // AIM
    // =========================

    private void HandleAimCamera()
    {
        if (input.AimPressed)
        {
            ShootCamera();
        }
        else
        {
            NormalCamera();
        }

        float targetFov =
            input.AimPressed
            ? aimFov
            : normalFov;

        mainCamera.fieldOfView =
            Mathf.Lerp(
                mainCamera.fieldOfView,
                targetFov,
                fovSpeed * Time.deltaTime);
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