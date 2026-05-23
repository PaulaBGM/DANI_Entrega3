using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }

    public bool ShootPressed { get; private set; }
    public bool AimPressed { get; private set; }

    public bool JumpPressed { get; private set; }

    // NUEVO
    public bool RunPressed { get; private set; }

    public bool DashPressed { get; private set; }

    public bool CrouchPressed { get; private set; }

    public bool InteractPressed { get; private set; }
    public bool InteractTriggered { get; private set; }

    public bool ScrollUpTriggered { get; private set; }
    public bool ScrollDownTriggered { get; private set; }

    private MyInputs inputActions;

    private void Awake()
    {
        inputActions = new MyInputs();
    }

    private void OnEnable()
    {
        inputActions.Enable();

        inputActions.Player.Move.performed +=
            ctx => MoveInput = ctx.ReadValue<Vector2>();

        inputActions.Player.Move.canceled +=
            ctx => MoveInput = Vector2.zero;

        inputActions.Player.Look.performed +=
            ctx => LookInput = ctx.ReadValue<Vector2>();

        inputActions.Player.Look.canceled +=
            ctx => LookInput = Vector2.zero;

        inputActions.Player.Shoot.performed +=
            ctx => ShootPressed = true;

        inputActions.Player.Shoot.canceled +=
            ctx => ShootPressed = false;

        inputActions.Player.Aim.performed +=
            ctx => AimPressed = true;

        inputActions.Player.Aim.canceled +=
            ctx => AimPressed = false;

        inputActions.Player.Jump.performed +=
            ctx => JumpPressed = true;

        inputActions.Player.Jump.canceled +=
            ctx => JumpPressed = false;

        // RUN
        inputActions.Player.Run.performed +=
            ctx => RunPressed = true;

        inputActions.Player.Run.canceled +=
            ctx => RunPressed = false;

        // DASH
        inputActions.Player.Dash.performed +=
            ctx => DashPressed = true;

        inputActions.Player.Dash.canceled +=
            ctx => DashPressed = false;

        inputActions.Player.Crouch.performed +=
            ctx => CrouchPressed = true;

        inputActions.Player.Crouch.canceled +=
            ctx => CrouchPressed = false;

        inputActions.Player.Interact.performed +=
            ctx =>
            {
                InteractPressed = true;

                InteractTriggered = true;
            };

        inputActions.Player.Interact.canceled +=
            ctx =>
            {
                InteractPressed = false;
            };

        inputActions.Player.ChangeWeapon.performed +=
            ctx =>
            {
                Vector2 scroll =
                    ctx.ReadValue<Vector2>();

                if (scroll.y > 0)
                {
                    ScrollUpTriggered = true;
                }

                if (scroll.y < 0)
                {
                    ScrollDownTriggered = true;
                }
            };
    }

    private void LateUpdate()
    {
        InteractTriggered = false;

        ScrollUpTriggered = false;

        ScrollDownTriggered = false;
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }
}