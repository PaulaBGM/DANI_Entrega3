using System;
using UnityEngine;

public class PlayerBehavior : BaseHealth, ITargeteable
{
    protected CharacterController ch_Controller;

    private PlayerMove playerMove;
    private WeaponManager weaponManager;

    [SerializeField] private float delayOpenGameOver;
    [SerializeField] private GameObject gameOverMenu;

    public Transform chestBone;

    public delegate void OnGameOverHandler();
    public static event OnGameOverHandler OnGameOver;

    public static event Action OnPlayerDied;

    public bool isInDialogue = false;

    protected override void Start()
    {
        base.Start();

        ch_Controller = GetComponent<CharacterController>();

        playerMove = GetComponent<PlayerMove>();
        weaponManager = GetComponent<WeaponManager>();
    }

    private void Update()
    {
        if (isInDialogue) return;

        bool hasWeapon =
            weaponManager != null &&
            weaponManager.CurrentWeaponType != WeaponType.Knife;

        playerMove.canLongIddle = !hasWeapon;

        animator.SetBool("hasWeapon", hasWeapon);

        animator.SetBool(
            "heavyWeapon",
            weaponManager != null &&
            weaponManager.IsHeavyWeaponEquipped);
    }

    protected override void Die()
    {
        if (isDead) return;

        isDead = true;

        Debug.Log("PLAYER DEAD");

        animator.SetBool("isDeath", true);

        OnGameOver?.Invoke();
        OnPlayerDied?.Invoke();

        if (gameOverMenu != null)
        {
            Invoke(nameof(OpenGameOver), delayOpenGameOver);
        }
    }

    private void OpenGameOver()
    {
        gameOverMenu.SetActive(true);
    }
}