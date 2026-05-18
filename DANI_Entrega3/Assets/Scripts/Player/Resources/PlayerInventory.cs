using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("Owned Weapons")]
    [SerializeField]
    private List<WeaponBase> ownedWeapons =
        new();

    [Header("References")]
    [SerializeField]
    private Animator animator;

    private PlayerInputHandler input;

    private int currentIndex;

    public WeaponBase CurrentWeapon
    {
        get;
        private set;
    }

    // Animator hashes
    private static readonly int LongWeapon =
        Animator.StringToHash("longWeapon");

    private static readonly int ShortWeapon =
        Animator.StringToHash("shortWeapon");

    private void Awake()
    {
        input =
            GetComponent<PlayerInputHandler>();

        if (animator == null)
        {
            animator =
                GetComponent<Animator>();
        }
    }

    private void Start()
    {
        // Desactivar TODAS las armas
        WeaponBase[] allWeapons =
            GetComponentsInChildren<WeaponBase>(true);

        foreach (WeaponBase weapon in allWeapons)
        {
            weapon.gameObject.SetActive(false);
        }

        // Activar arma inicial (Knife)
        if (ownedWeapons.Count > 0)
        {
            EquipWeapon(0);

            Debug.Log(
                $"START WEAPON: {ownedWeapons[0].weaponType}");
        }
    }

    private void Update()
    {
        HandleScroll();
    }

    private void HandleScroll()
    {
        if (input == null)
            return;

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
    // ADD WEAPON
    // =========================

    public void AddWeapon(WeaponType type)
    {
        // Ya la tiene
        foreach (WeaponBase owned in ownedWeapons)
        {
            if (owned.weaponType == type)
            {
                Debug.Log(
                    $"ALREADY OWNED: {type}");

                return;
            }
        }

        // Buscar en el rig del jugador
        WeaponBase[] allWeapons =
            GetComponentsInChildren<WeaponBase>(true);

        foreach (WeaponBase weapon in allWeapons)
        {
            if (weapon.weaponType != type)
                continue;

            ownedWeapons.Add(weapon);

            weapon.gameObject.SetActive(true);

            EquipWeapon(
                ownedWeapons.Count - 1);

            Debug.Log(
                $"WEAPON ADDED: {type}");

            return;
        }

        Debug.LogError(
            $"NO WEAPON FOUND IN RIG: {type}");
    }

    // =========================
    // NEXT WEAPON
    // =========================

    public void NextWeapon()
    {
        if (ownedWeapons.Count <= 1)
            return;

        currentIndex++;

        if (currentIndex >= ownedWeapons.Count)
        {
            currentIndex = 0;
        }

        EquipWeapon(currentIndex);
    }

    // =========================
    // PREVIOUS WEAPON
    // =========================

    public void PreviousWeapon()
    {
        if (ownedWeapons.Count <= 1)
            return;

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
        if (ownedWeapons.Count <= 0)
            return;

        currentIndex = index;

        for (int i = 0; i < ownedWeapons.Count; i++)
        {
            if (ownedWeapons[i] != null)
            {
                ownedWeapons[i]
                    .gameObject
                    .SetActive(i == index);
            }
        }

        CurrentWeapon =
            ownedWeapons[index];

        UpdateAnimator(
            CurrentWeapon.weaponType);

        Debug.Log(
            $"EQUIPPED: {CurrentWeapon.weaponType}");
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

            case WeaponType.Short:

                animator.SetBool(
                    ShortWeapon,
                    true);

                break;
        }
    }
}