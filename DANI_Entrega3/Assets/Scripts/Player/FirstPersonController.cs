using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    private static readonly int Speed =
        Animator.StringToHash("Speed");

    private static readonly int Death =
        Animator.StringToHash("Death");

    [SerializeField] private Animator animator;

    [SerializeField]
    private CinemachineInputAxisController cinnemachineInput;

    [Header("Movement")]
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpForce = 5f;

    [Header("Ground Check")]
    [SerializeField] private Transform feet;
    [SerializeField] private float detectionRadius = 0.2f;
    [SerializeField] private LayerMask isGround;

    private PlayerMain playerMain;

    private Vector3 verticalMove;

    private CharacterController ch_Controller;

    private Camera mainCamera;

    private bool isGrounded;

    private Vector2 movementInput;

    private void Awake()
    {
        playerMain =
            GetComponentInParent<PlayerMain>();

        ch_Controller =
            GetComponent<CharacterController>();

        mainCamera = Camera.main;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnEnable()
    {
        InputController.Instance.OnJump += Jump;

        InputController.Instance.OnMove +=
            UpdateMovement;

        playerMain.OnPlayerDeath += OnDeath;
    }

    private void OnDisable()
    {
        if (InputController.Instance != null)
        {
            InputController.Instance.OnJump -= Jump;

            InputController.Instance.OnMove -=
                UpdateMovement;
        }

        playerMain.OnPlayerDeath -= OnDeath;
    }

    private void UpdateMovement(Vector2 inputVector)
    {
        movementInput = inputVector;
    }

    private void Update()
    {
        if (UIManager.Instance != null &&
            UIManager.Instance.IsDialogueActive)
            return;

        GroundCheck();

        ApplyGravity();

        MoveAndRotate();
    }

    private void GroundCheck()
    {
        isGrounded =
            Physics.CheckSphere(
                feet.position,
                detectionRadius,
                isGround);
    }

    private void ApplyGravity()
    {
        if (isGrounded && verticalMove.y < 0)
        {
            verticalMove.y = -1f;
        }
        else
        {
            verticalMove.y += gravity * Time.deltaTime;
        }

        ch_Controller.Move(
            verticalMove * Time.deltaTime);
    }

    private void MoveAndRotate()
    {
        // Rotar cuerpo hacia cámara
        transform.rotation =
            Quaternion.Euler(
                0,
                mainCamera.transform.rotation.eulerAngles.y,
                0);

        if (movementInput.sqrMagnitude > 0)
        {
            float angleToRotate =
                Mathf.Atan2(
                    movementInput.x,
                    movementInput.y)
                * Mathf.Rad2Deg
                + mainCamera.transform.eulerAngles.y;

            Vector3 movementVector =
                Quaternion.Euler(0, angleToRotate, 0)
                * Vector3.forward;

            ch_Controller.Move(
                movementVector *
                movementSpeed *
                Time.deltaTime);
        }

        animator.SetFloat(
            Speed,
            ch_Controller.velocity.magnitude);
    }

    private void Jump()
    {
        if (isGrounded &&
            UIManager.Instance != null &&
            !UIManager.Instance.IsDialogueActive)
        {
            verticalMove.y =
                Mathf.Sqrt(-2 * jumpForce * gravity);
        }
    }

    // Animation Event
    public void WalkSound()
    {
        AudioManager.Instance.PlaySFX(
            AudioManager.Instance.audioLibrary.stepSfx);
    }

    private void OnDeath()
    {
        animator.SetTrigger(Death);

        enabled = false;

        cinnemachineInput.enabled = false;
    }

    private void OnDrawGizmos()
    {
        if (feet == null)
            return;

        Gizmos.color = Color.green;

        Gizmos.DrawSphere(
            feet.position,
            detectionRadius);
    }
}