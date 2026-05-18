using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    private PlayerInputHandler input;
    private PlayerMotor motor;

    private static readonly int ZSpeed =
        Animator.StringToHash("zSpeed");

    private static readonly int XSpeed =
        Animator.StringToHash("xSpeed");

    private static readonly int Jump =
        Animator.StringToHash("jump");

    private static readonly int Crouched =
        Animator.StringToHash("crouched");

    private static readonly int LongIdle =
        Animator.StringToHash("longIdle");

    private static readonly int Movement =
        Animator.StringToHash("movement");

    private float idleTimer;

    [SerializeField]
    private float longIdleTime = 15f;

    private void Awake()
    {
        input = GetComponent<PlayerInputHandler>();

        motor = GetComponent<PlayerMotor>();

        // Seguridad
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    private void Update()
    {
        UpdateMovement();

        UpdateCrouch();

        UpdateLongIdle();
    }

    private void UpdateMovement()
    {
        Vector2 move = input.MoveInput;

        float speed;

        if (motor.IsCrouching)
        {
            speed = 1f;
        }
        else if (motor.IsRunning)
        {
            speed = 5f;
        }
        else
        {
            speed = 3f;
        }

        float xSpeed = move.x * speed;

        float zSpeed = move.y * speed;

        animator.SetFloat(XSpeed, xSpeed);

        animator.SetFloat(ZSpeed, zSpeed);

        bool moving =
            Mathf.Abs(move.x) > 0.1f ||
            Mathf.Abs(move.y) > 0.1f;

        animator.SetBool(Movement, moving);
    }

    private void UpdateCrouch()
    {
        if (motor.IsCrouching)
        {
            animator.SetInteger(Crouched, 1);
        }
        else
        {
            animator.SetInteger(Crouched, 2);
        }
    }

    private void UpdateLongIdle()
    {
        bool moving =
            Mathf.Abs(input.MoveInput.x) > 0.1f ||
            Mathf.Abs(input.MoveInput.y) > 0.1f;

        if (moving)
        {
            idleTimer = 0f;

            animator.SetBool(LongIdle, false);

            return;
        }

        idleTimer += Time.deltaTime;

        if (idleTimer >= longIdleTime)
        {
            animator.SetBool(LongIdle, true);
        }
    }

    public void PlayJumpStart()
    {
        animator.SetInteger(Jump, 1);
    }

    public void PlayLanding()
    {
        animator.SetInteger(Jump, 2);
    }

    public void ResetJump()
    {
        animator.SetInteger(Jump, 0);
    }
}