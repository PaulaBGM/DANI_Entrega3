using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    private CharacterController ch_Controller;
    private PlayerBehavior playerBehavior;
    private WeaponManager weaponManager;

    private float gravity = -9.8f;

    [Header("Movement")]
    [SerializeField] private float normalSpeed = 3f;
    [SerializeField] private float currentSpeed = 0f;
    [SerializeField] private float runSpeed = 5f;
    [SerializeField] private float underWaterSpeed = 1.5f;
    [SerializeField] private float stickToGroundSpeed = -3f;

    [Header("Jump")]
    private float jumpTimer = 0f;

    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float endJumpAnimTime = 1.5f;
    [SerializeField] private float timeBetweenJump = 0.5f;
    [SerializeField] private float initialJumpAnimTime;

    private float startJumpAnimTime;

    [Header("Slide")]
    [SerializeField] private AnimationCurve slideSlowCurve;
    [SerializeField] private float slideSlope = 4f;
    [SerializeField] private float slideSpeed = 3f;
    [SerializeField] private float maxSlideVelocity = 6f;
    [SerializeField] private float slideDownTime = 3f;

    [Header("Crouched")]
    [SerializeField] private float crouchSpeed = 1f;
    [SerializeField] private float standHeight = 2f;
    [SerializeField] private float crouchHeight = 0.8f;
    [SerializeField] private float crouchCenter = 0.4f;
    [SerializeField] private float standCenter = 0.5f;
    [SerializeField] private float endCrouchAnimTime = 1.5f;

    [Header("LongIdle")]
    [SerializeField] private float longIdleTime = 15f;

    [Header("Dash")]
    [SerializeField] private float dashDuration = 1f;
    [SerializeField] private float dashSpeed = 7f;

    [Header("Layers")]
    [SerializeField] private LayerMask ceilingLayer;

    private Vector3 playerVelocity;
    private Vector3 slideVelocity;
    private Vector3 dashDirection;

    private float verticalVelocity;
    private float longIdleTimer = 0f;
    private float slidenTime = 0f;
    private float slideVelocityFactor = 1f;
    private float dashTime;

    private bool isJumping;
    private bool walking = false;
    private bool waitingForJumpAnim = false;
    private bool endJump = true;

    private bool isCrouched = false;
    private bool tryingToStand = false;

    private bool sliding = false;
    private bool isInWater = false;
    private bool isDashing = false;

    public bool canLongIddle;

    private static readonly int Jump = Animator.StringToHash("jump");
    private static readonly int ZSpeed = Animator.StringToHash("zSpeed");
    private static readonly int XSpeed = Animator.StringToHash("xSpeed");
    private static readonly int Crouched = Animator.StringToHash("crouched");

    private void Start()
    {
        ch_Controller = GetComponent<CharacterController>();
        playerBehavior = GetComponent<PlayerBehavior>();
        weaponManager = GetComponent<WeaponManager>();
    }

    private void Update()
    {
        if (playerBehavior.IsDead) return;

        if (isDashing)
        {
            HandleDash();
            return;
        }

        startJumpAnimTime =
            walking ? 0 : initialJumpAnimTime;

        UpdatePlayerVelocity();
        DoJump();
        UpdateSlideVelocity();
        ApplyVelocity();

        // SOLO bloquear crouch con armas pesadas
        if (weaponManager == null ||
            !weaponManager.IsHeavyWeaponEquipped)
        {
            HandleCrouch();
        }

        if (tryingToStand)
        {
            TryStandUp();
        }

        if (Keyboard.current.cKey.wasPressedThisFrame)
        {
            StartDash();
        }

        LongIdle();
    }

    private void LongIdle()
    {
        if (!canLongIddle) return;

        if (playerVelocity.sqrMagnitude > 0.01f ||
            isJumping ||
            isDashing ||
            isCrouched)
        {
            longIdleTimer = 0f;

            playerBehavior.Animator.SetBool("longIdle", false);
            playerBehavior.Animator.SetBool("movement", true);

            return;
        }

        longIdleTimer += Time.deltaTime;

        if (longIdleTimer > longIdleTime)
        {
            playerBehavior.Animator.SetBool("longIdle", true);
            playerBehavior.Animator.SetBool("movement", false);
        }
    }

    private void ApplyVelocity()
    {
        Vector3 horizontalVelocity =
            playerVelocity + slideVelocity;

        if (!isInWater)
        {
            horizontalVelocity *= slideVelocityFactor;
        }

        Vector3 totalVelocity =
            horizontalVelocity +
            Vector3.up * verticalVelocity;

        ch_Controller.Move(totalVelocity * Time.deltaTime);
    }

    private void HandleCrouch()
    {
        if (Keyboard.current.leftCtrlKey.wasPressedThisFrame)
        {
            if (!isCrouched && !tryingToStand)
            {
                StartCrouch();
            }
            else if (isCrouched && !tryingToStand)
            {
                tryingToStand = true;
            }
        }
    }

    private void UpdatePlayerVelocity()
    {
        Vector2 movementInput = Vector2.zero;

        if (Keyboard.current.wKey.isPressed) movementInput.y += 1;
        if (Keyboard.current.sKey.isPressed) movementInput.y -= 1;
        if (Keyboard.current.aKey.isPressed) movementInput.x -= 1;
        if (Keyboard.current.dKey.isPressed) movementInput.x += 1;

        Vector3 vectorInput =
            new Vector3(movementInput.x, 0, movementInput.y);

        walking = vectorInput.sqrMagnitude > 0.01f;

        if (vectorInput.sqrMagnitude > 1)
        {
            vectorInput.Normalize();
        }

        if (isCrouched)
        {
            currentSpeed = crouchSpeed;
        }
        else if (isInWater)
        {
            currentSpeed = underWaterSpeed;
        }
        else
        {
            currentSpeed =
                Keyboard.current.leftShiftKey.isPressed
                ? runSpeed
                : normalSpeed;
        }

        Vector3 localVelocity =
            new Vector3(
                vectorInput.x * currentSpeed,
                0,
                vectorInput.z * currentSpeed
            );

        playerVelocity =
            transform.TransformVector(localVelocity);

        playerBehavior.Animator.SetFloat(ZSpeed, localVelocity.z);
        playerBehavior.Animator.SetFloat(XSpeed, localVelocity.x);
    }

    private void DoJump()
    {
        jumpTimer += Time.deltaTime;

        if (!ch_Controller.isGrounded)
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        if (Keyboard.current.spaceKey.wasPressedThisFrame &&
            ch_Controller.isGrounded &&
            !isJumping &&
            !waitingForJumpAnim &&
            endJump &&
            jumpTimer > timeBetweenJump)
        {
            isJumping = true;
            waitingForJumpAnim = true;

            playerBehavior.Animator.SetInteger(Jump, 1);

            StartCoroutine(JumpCoroutine());
        }

        if (ch_Controller.isGrounded &&
            !endJump &&
            !isJumping)
        {
            endJump = true;
            jumpTimer = 0;

            playerBehavior.Animator.SetInteger(Jump, 0);

            verticalVelocity = stickToGroundSpeed;
        }
    }

    private IEnumerator JumpCoroutine()
    {
        yield return new WaitForSeconds(startJumpAnimTime);

        verticalVelocity = jumpForce;

        endJump = false;

        StartCoroutine(EndJumpCoroutine());
    }

    private IEnumerator EndJumpCoroutine()
    {
        yield return new WaitForSeconds(endJumpAnimTime);

        playerBehavior.Animator.SetInteger(Jump, 2);

        isJumping = false;
        waitingForJumpAnim = false;
    }

    private void UpdateSlideVelocity()
    {
        RaycastHit hitInfo;

        Vector3 rayOrigin =
            transform.position + Vector3.up * 0.1f;

        bool hit = Physics.Raycast(
            rayOrigin,
            Vector3.down,
            out hitInfo,
            ch_Controller.height / 2 + 0.5f);

        if (ch_Controller.isGrounded && hit)
        {
            float angle =
                Vector3.Angle(hitInfo.normal, Vector3.up);

            if (angle > slideSlope)
            {
                if (!sliding)
                {
                    sliding = true;
                    slidenTime = 0f;
                    slideVelocity = Vector3.zero;
                }

                Vector3 slideDir =
                    Vector3.ProjectOnPlane(
                        Vector3.down,
                        hitInfo.normal
                    ).normalized;

                Vector3 targetSlide =
                    slideDir * slideSpeed;

                targetSlide =
                    Vector3.ClampMagnitude(
                        targetSlide,
                        maxSlideVelocity);

                slideVelocity =
                    Vector3.Lerp(
                        slideVelocity,
                        targetSlide,
                        Time.deltaTime * 5f);
            }
            else
            {
                slideVelocity =
                    Vector3.Lerp(
                        slideVelocity,
                        Vector3.zero,
                        Time.deltaTime * 10f);

                if (slideVelocity.magnitude < 0.1f)
                {
                    sliding = false;
                    slideVelocity = Vector3.zero;
                }
            }
        }

        if (sliding)
        {
            slidenTime += Time.deltaTime;

            float t =
                Mathf.Clamp01(slidenTime / slideDownTime);

            slideVelocityFactor =
                Mathf.Max(
                    0.4f,
                    slideSlowCurve.Evaluate(t));
        }
        else
        {
            slideVelocityFactor =
                Mathf.Lerp(
                    slideVelocityFactor,
                    1f,
                    Time.deltaTime * 10f);
        }
    }

    private void StartCrouch()
    {
        isCrouched = true;

        ch_Controller.height = crouchHeight;
        ch_Controller.center =
            new Vector3(0, crouchCenter, 0);

        playerBehavior.Animator.SetInteger(Crouched, 1);

        tryingToStand = false;
    }

    private void TryStandUp()
    {
        if (CanStandUp())
        {
            StandUp();
            tryingToStand = false;
        }
    }

    private bool CanStandUp()
    {
        RaycastHit hitInfo;

        float checkDistance =
            standHeight - crouchHeight;

        Vector3 start =
            transform.position + Vector3.up * crouchHeight;

        return !Physics.SphereCast(
            start,
            ch_Controller.radius,
            Vector3.up,
            out hitInfo,
            checkDistance,
            ceilingLayer);
    }

    private void StandUp()
    {
        ch_Controller.height = standHeight;

        ch_Controller.center =
            new Vector3(0, standCenter, 0);

        playerBehavior.Animator.SetInteger(Crouched, 2);

        isCrouched = false;

        StartCoroutine(ResetCrouchState());
    }

    private IEnumerator ResetCrouchState()
    {
        yield return new WaitForSeconds(endCrouchAnimTime);

        playerBehavior.Animator.SetInteger(Crouched, 0);
    }

    private void StartDash()
    {
        if (isInWater) return;

        isDashing = true;
        dashTime = 0;

        dashDirection =
            playerVelocity.sqrMagnitude > 0
            ? playerVelocity.normalized
            : transform.forward;
    }

    private void HandleDash()
    {
        dashTime += Time.deltaTime;

        playerBehavior.Animator.SetFloat(ZSpeed, dashSpeed);

        ch_Controller.Move(
            dashDirection *
            dashSpeed *
            Time.deltaTime);

        if (dashTime >= dashDuration)
        {
            isDashing = false;
        }
    }

    public void SetUnderwaterSpeed(bool inWater)
    {
        isInWater = inWater;
    }
}