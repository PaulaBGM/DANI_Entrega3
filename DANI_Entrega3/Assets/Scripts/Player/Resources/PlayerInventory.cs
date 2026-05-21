using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("Owned Weapons")]
    [SerializeField] private List<WeaponHolder> ownedWeapons = new();

    [Header("References")]
    [SerializeField] private Animator animator;

    private PlayerInputHandler input;

    private int currentIndex;

    public WeaponHolder CurrentWeapon { get; private set; }

    private static readonly int LongWeapon = Animator.StringToHash("longWeapon");
    private static readonly int ShortWeapon = Animator.StringToHash("shortWeapon");

    private void Awake()
    {
        input = GetComponentInParent<PlayerInputHandler>();

        if (animator == null)
            animator = GetComponent<Animator>();

        WeaponHolder[] allWeapons = GetComponentsInChildren<WeaponHolder>(true);

        foreach (WeaponHolder holder in allWeapons)
        {
            if (holder.weaponBase == null)
            {
                Debug.LogError($"WEAPON BASE NULL ON: {holder.name}");
                continue;
            }

        }
    }

    private void Start()
    {

        WeaponHolder[] allWeapons = GetComponentsInChildren<WeaponHolder>(true);

        foreach (WeaponHolder holder in allWeapons)
        {

            holder.gameObject.SetActive(false);
        }

        if (ownedWeapons.Count > 0)
        {
            ownedWeapons[0].gameObject.SetActive(true);

            EquipWeapon(0);
        }
    }

    private void Update()
    {
        if (input == null)
        {
            return;
        }

        HandleScroll();
    }

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

    public void AddWeapon(WeaponType type)
    {

        foreach (WeaponHolder owned in ownedWeapons)
        {

            if (owned.weaponType == type)
            {
                return;
            }
        }

        WeaponHolder[] allWeapons = GetComponentsInChildren<WeaponHolder>(true);

        foreach (WeaponHolder holder in allWeapons)
        {

            if (holder.weaponType != type)
                continue;

            ownedWeapons.Add(holder);

            holder.gameObject.SetActive(true);

            EquipWeapon(ownedWeapons.Count - 1);

            Debug.Log($"WEAPON ADDED: {type}");

            return;
        }

    }

    public void NextWeapon()
    {
        if (ownedWeapons.Count <= 1)
        {
            Debug.Log("NOT ENOUGH WEAPONS");
            return;
        }

        currentIndex++;

        if (currentIndex >= ownedWeapons.Count)
            currentIndex = 0;

        EquipWeapon(currentIndex);
    }

    public void PreviousWeapon()
    {
        if (ownedWeapons.Count <= 1)
        {
            Debug.Log("NOT ENOUGH WEAPONS");
            return;
        }

        currentIndex--;

        if (currentIndex < 0)
            currentIndex = ownedWeapons.Count - 1;

        EquipWeapon(currentIndex);
    }

    private void EquipWeapon(int index)
    {
        if (ownedWeapons.Count <= 0)
            return;

        currentIndex = index;

        for (int i = 0; i < ownedWeapons.Count; i++)
        {
            if (ownedWeapons[i] == null)
                continue;

            bool equipped =
                i == index;

            WeaponHolder holder =
                ownedWeapons[i];

            // HAND WEAPON
            if (holder.handWeapon != null)
            {
                holder.handWeapon.SetActive(
                    equipped);
            }

            // BACK WEAPON
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

        Debug.Log(
            $"EQUIPPED: {CurrentWeapon.weaponType}");
    }

    private void UpdateAnimator(WeaponType type)
    {
        animator.SetBool(LongWeapon, false);
        animator.SetBool(ShortWeapon, false);

        switch (type)
        {
            case WeaponType.Long:
                animator.SetBool(LongWeapon, true);
                break;

            case WeaponType.GrenadeLauncher:
                animator.SetBool(LongWeapon, true);
                break;

            case WeaponType.Short:
                animator.SetBool(ShortWeapon, true);
                break;
        }
    }
}