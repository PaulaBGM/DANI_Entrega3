using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMotor : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float runSpeed = 5f;
    [SerializeField] private float crouchSpeed = 1f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float gravity = -20f;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 12f;
    [SerializeField] private float dashDuration = 0.25f;

    [Header("Crouch")]
    [SerializeField] private float standHeight = 2f;
    [SerializeField] private float crouchHeight = 0.8f;

    [SerializeField] private float standCenter = 1f;
    [SerializeField] private float crouchCenter = 0.4f;

    private CharacterController controller;
    private PlayerInputHandler input;
    private PlayerAnimator playerAnimator;

    private Vector3 velocity;

    private bool isDashing;
    private float dashTimer;

    private bool isCrouching;

    private bool crouchPressedLastFrame;

    // LANDING DETECTION
    private bool wasGrounded;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        input = GetComponent<PlayerInputHandler>();

        playerAnimator = GetComponent<PlayerAnimator>();
    }

    private void Update()
    {
        DebugStates();

        HandleCrouchToggle();

        HandleMovement();

        HandleJump();

        HandleDash();

        ApplyCrouch();
    }

    private void HandleMovement()
    {
        Vector2 moveInput = input.MoveInput;

        Vector3 move =
            transform.right * moveInput.x +
            transform.forward * moveInput.y;

        float currentSpeed = walkSpeed;

        // NO correr mientras está agachado
        if (isCrouching)
        {
            currentSpeed = crouchSpeed;
        }
        else if (input.RunPressed)
        {
            currentSpeed = runSpeed;
        }

        bool grounded = controller.isGrounded;

        if (grounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // LANDING DETECTION
        if (!wasGrounded && grounded)
        {
            Debug.Log("LANDED");

            playerAnimator.PlayLanding();
        }

        wasGrounded = grounded;

        // APPLY GRAVITY
        velocity.y += gravity * Time.deltaTime;

        Vector3 finalMove =
            move * currentSpeed;

        finalMove.y = velocity.y;

        // NO mover normalmente durante dash
        if (!isDashing)
        {
            controller.Move(
                finalMove * Time.deltaTime);
        }
    }

    private void HandleJump()
    {
        if (!input.JumpPressed)
            return;

        if (!controller.isGrounded)
            return;

        Debug.Log("JUMP START");

        velocity.y = jumpForce;

        playerAnimator.PlayJumpStart();
    }

    private void HandleDash()
    {
        // NO dash mientras está agachado
        if (input.DashPressed &&
            !isDashing &&
            !isCrouching)
        {
            Debug.Log("DASH START");

            isDashing = true;
            dashTimer = dashDuration;
        }

        if (!isDashing)
            return;

        Vector3 dashDirection =
            transform.forward;

        controller.Move(
            dashDirection *
            dashSpeed *
            Time.deltaTime);

        dashTimer -= Time.deltaTime;

        if (dashTimer <= 0)
        {
            Debug.Log("DASH END");

            isDashing = false;
        }
    }

    private void HandleCrouchToggle()
    {
        if (input.CrouchPressed &&
            !crouchPressedLastFrame)
        {
            isCrouching = !isCrouching;

            Debug.Log(
                $"CROUCH TOGGLED: {isCrouching}");
        }

        crouchPressedLastFrame =
            input.CrouchPressed;
    }

    private void ApplyCrouch()
    {
        if (isCrouching)
        {
            controller.height = crouchHeight;

            controller.center =
                new Vector3(
                    0,
                    crouchCenter,
                    0);
        }
        else
        {
            controller.height = standHeight;

            controller.center =
                new Vector3(
                    0,
                    standCenter,
                    0);
        }
    }

    private void DebugStates()
    {
        if (Keyboard.current.f1Key.wasPressedThisFrame)
        {
            Debug.Log(
                $"isGrounded: {controller.isGrounded}");

            Debug.Log(
                $"isCrouching: {isCrouching}");

            Debug.Log(
                $"isDashing: {isDashing}");

            Debug.Log(
                $"velocityY: {velocity.y}");

            Debug.Log(
                $"controllerHeight: {controller.height}");

            Debug.Log(
                $"controllerCenter: {controller.center}");
        }
    }

    public bool IsCrouching =>
        isCrouching;

    public bool IsRunning => input.RunPressed && !isCrouching;
}