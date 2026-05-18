using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerBehavior : BaseHealth, ITargeteable
{
    protected CharacterController ch_Controller;

    private PlayerMove playerMove;

    [SerializeField] private float delayOpenGameOver;
    [SerializeField] private GameObject gameOverMenu;

    private GetWeapon getWeapon;

    public Transform chestBone;

    // Se añade un delegado para notificar al UI
    public delegate void OnGameOverHandler();
    public static event OnGameOverHandler OnGameOver;

    public bool isInDialogue = false;

    public static event Action OnPlayerDied;

    protected override void Start()
    {
        base.Start();

        playerMove = GetComponent<PlayerMove>();
        getWeapon = GetComponent<GetWeapon>();
    }

    private void Update()
    {
        if (isInDialogue) return;

        if (getWeapon.hasPistol || getWeapon.hasLargeWeapon)
        {
            playerMove.canLongIddle = false;
            animator.SetBool("hasPistol", getWeapon.hasPistol);
        }
        else
        {
            playerMove.canLongIddle = true;
            animator.SetBool("hasPistol", getWeapon.hasPistol);
        }
    }

    protected override void Die()
    {
        if (isDead) return;

        isDead = true;

        animator.SetBool("isDeath", true);

        OnGameOver?.Invoke();
        OnPlayerDied?.Invoke();
    }
}