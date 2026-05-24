using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("Owned Weapons")]
    [SerializeField]
    private List<WeaponHolder> ownedWeapons =
        new();

    [Header("References")]
    [SerializeField]
    private Animator animator;

    private PlayerInputHandler input;

    private int currentIndex;

    public WeaponHolder CurrentWeapon
    {
        get;
        private set;
    }

    private static readonly int LongWeapon =
        Animator.StringToHash("longWeapon");

    private static readonly int ShortWeapon =
        Animator.StringToHash("shortWeapon");

    private void Awake()
    {
        input =
            GetComponentInParent<PlayerInputHandler>();

        if (animator == null)
        {
            animator =
                GetComponent<Animator>();
        }
    }

    private void Start()
    {
        if (ownedWeapons.Count <= 0)
            return;

        EquipWeapon(0);
    }

    private void Update()
    {
        HandleScroll();
    }

    // =========================
    // SCROLL
    // =========================

    private void HandleScroll()
    {
        if (input.ScrollUpTriggered)
        {
            NextWeapon();
        }

        if (input.ScrollDownTriggered)
        {
            PreviousWeapon();
        }
    }

    // =========================
    // NEXT
    // =========================

    public void NextWeapon()
    {
        currentIndex++;

        if (currentIndex >= ownedWeapons.Count)
        {
            currentIndex = 0;
        }

        EquipWeapon(currentIndex);
    }

    // =========================
    // PREVIOUS
    // =========================

    public void PreviousWeapon()
    {
        currentIndex--;

        if (currentIndex < 0)
        {
            currentIndex =
                ownedWeapons.Count - 1;
        }

        EquipWeapon(currentIndex);
    }

    // =========================
    // EQUIP
    // =========================

    private void EquipWeapon(int index)
    {
        currentIndex = index;

        for (int i = 0; i < ownedWeapons.Count; i++)
        {
            bool equipped =
                i == index;

            WeaponHolder holder =
                ownedWeapons[i];

            if (holder.handWeapon != null)
            {
                holder.handWeapon.SetActive(
                    equipped);
            }

            if (holder.backWeapon != null)
            {
                holder.backWeapon.SetActive(
                    !equipped);
            }
        }

        CurrentWeapon =
            ownedWeapons[index];

        UpdateAnimator(
            CurrentWeapon.weaponType);
    }

    // =========================
    // ANIMATOR
    // =========================

    private void UpdateAnimator(
        WeaponType type)
    {
        animator.SetBool(
            LongWeapon,
            false);

        animator.SetBool(
            ShortWeapon,
            false);

        switch (type)
        {
            case WeaponType.Long:

                animator.SetBool(
                    LongWeapon,
                    true);

                break;

            case WeaponType.GrenadeLauncher:

                animator.SetBool(
                    LongWeapon,
                    true);

                break;

            case WeaponType.Short:

                animator.SetBool(
                    ShortWeapon,
                    true);

                break;
        }
    }
}