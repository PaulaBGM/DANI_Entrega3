using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    public static InputController Instance { get; private set; }

    // ---------- MOVEMENT ----------
    public event Action<Vector2> OnMove;
    public event Action OnJump;

    // ---------- COMBAT ----------
    public event Action OnShootPressed;
    public event Action OnShootReleased;

    public event Action OnAimPressed;
    public event Action OnAimReleased;

    public event Action<float> OnScrollWeapon;
    public event Action<int> OnSelectWeapon;

    public event Action OnReload;
    public event Action OnKnife;

    // ---------- INTERACTION ----------
    public event Action OnInteract;
    public event Action OnContinueDialogue;

    private PlayerInput playerInput;

    // InputActions cache
    private InputAction moveAction;
    private InputAction jumpAction;

    private InputAction shootAction;
    private InputAction aimAction;

    private InputAction scrollWeaponAction;

    private InputAction weapon1Action;
    private InputAction weapon2Action;
    private InputAction weapon3Action;
    private InputAction weapon4Action;

    private InputAction reloadAction;
    private InputAction knifeAction;

    private InputAction interactAction;
    private InputAction continueAction;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            playerInput = GetComponent<PlayerInput>();

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // ---------- ACTION REFERENCES ----------

        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];

        shootAction = playerInput.actions["Shoot"];
        aimAction = playerInput.actions["Aim"];

        scrollWeaponAction = playerInput.actions["ChangeWeapon"];

        weapon1Action = playerInput.actions["Weapon1"];
        weapon2Action = playerInput.actions["Weapon2"];
        weapon3Action = playerInput.actions["Weapon3"];
        weapon4Action = playerInput.actions["Weapon4"];

        reloadAction = playerInput.actions["Reload"];
        knifeAction = playerInput.actions["Knife"];

        interactAction = playerInput.actions["Interact"];
        continueAction = playerInput.actions["Continue"];
    }

    private void OnEnable()
    {
        // ---------- MOVE ----------

        moveAction.performed += OnMovePerformed;
        moveAction.canceled += OnMoveCanceled;

        jumpAction.started += OnJumpStarted;

        // ---------- SHOOT ----------

        shootAction.started += OnShootStarted;
        shootAction.canceled += OnShootCanceled;

        // ---------- AIM ----------

        aimAction.started += OnAimStarted;
        aimAction.canceled += OnAimCanceled;

        // ---------- WEAPON SWITCH ----------

        scrollWeaponAction.performed += OnScrollPerformed;

        weapon1Action.started += OnWeapon1;
        weapon2Action.started += OnWeapon2;
        weapon3Action.started += OnWeapon3;
        weapon4Action.started += OnWeapon4;

        // ---------- OTHER COMBAT ----------

        reloadAction.started += OnReloadStarted;
        knifeAction.started += OnKnifeStarted;

        // ---------- INTERACTION ----------

        interactAction.started += OnInteractStarted;
        continueAction.started += OnContinueStarted;
    }

    // =========================================================
    // MOVE
    // =========================================================

    private void OnMovePerformed(InputAction.CallbackContext ctx)
    {
        OnMove?.Invoke(ctx.ReadValue<Vector2>());
    }

    private void OnMoveCanceled(InputAction.CallbackContext ctx)
    {
        OnMove?.Invoke(Vector2.zero);
    }

    private void OnJumpStarted(InputAction.CallbackContext ctx)
    {
        OnJump?.Invoke();
    }

    // =========================================================
    // SHOOT
    // =========================================================

    private void OnShootStarted(InputAction.CallbackContext ctx)
    {
        OnShootPressed?.Invoke();
    }

    private void OnShootCanceled(InputAction.CallbackContext ctx)
    {
        OnShootReleased?.Invoke();
    }

    // =========================================================
    // AIM
    // =========================================================

    private void OnAimStarted(InputAction.CallbackContext ctx)
    {
        OnAimPressed?.Invoke();
    }

    private void OnAimCanceled(InputAction.CallbackContext ctx)
    {
        OnAimReleased?.Invoke();
    }

    // =========================================================
    // WEAPON CHANGE
    // =========================================================

    private void OnScrollPerformed(InputAction.CallbackContext ctx)
    {
        OnScrollWeapon?.Invoke(ctx.ReadValue<float>());
    }

    private void OnWeapon1(InputAction.CallbackContext ctx)
    {
        OnSelectWeapon?.Invoke(0);
    }

    private void OnWeapon2(InputAction.CallbackContext ctx)
    {
        OnSelectWeapon?.Invoke(1);
    }

    private void OnWeapon3(InputAction.CallbackContext ctx)
    {
        OnSelectWeapon?.Invoke(2);
    }

    private void OnWeapon4(InputAction.CallbackContext ctx)
    {
        OnSelectWeapon?.Invoke(3);
    }

    // =========================================================
    // OTHER COMBAT
    // =========================================================

    private void OnReloadStarted(InputAction.CallbackContext ctx)
    {
        OnReload?.Invoke();
    }

    private void OnKnifeStarted(InputAction.CallbackContext ctx)
    {
        OnKnife?.Invoke();
    }

    // =========================================================
    // INTERACTION
    // =========================================================

    private void OnInteractStarted(InputAction.CallbackContext ctx)
    {
        OnInteract?.Invoke();
    }

    private void OnContinueStarted(InputAction.CallbackContext ctx)
    {
        OnContinueDialogue?.Invoke();
    }

    // =========================================================
    // DISABLE
    // =========================================================

    private void OnDisable()
    {
        // MOVE
        moveAction.performed -= OnMovePerformed;
        moveAction.canceled -= OnMoveCanceled;
        jumpAction.started -= OnJumpStarted;

        // SHOOT
        shootAction.started -= OnShootStarted;
        shootAction.canceled -= OnShootCanceled;

        // AIM
        aimAction.started -= OnAimStarted;
        aimAction.canceled -= OnAimCanceled;

        // WEAPON SWITCH
        scrollWeaponAction.performed -= OnScrollPerformed;

        weapon1Action.started -= OnWeapon1;
        weapon2Action.started -= OnWeapon2;
        weapon3Action.started -= OnWeapon3;
        weapon4Action.started -= OnWeapon4;

        // OTHER COMBAT
        reloadAction.started -= OnReloadStarted;
        knifeAction.started -= OnKnifeStarted;

        // INTERACTION
        interactAction.started -= OnInteractStarted;
        continueAction.started -= OnContinueStarted;
    }
}