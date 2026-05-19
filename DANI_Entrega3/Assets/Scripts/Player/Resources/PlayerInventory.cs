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
        Debug.Log("PLAYER INVENTORY AWAKE");

        input = GetComponentInParent<PlayerInputHandler>();

        if (animator == null)
            animator = GetComponent<Animator>();

        WeaponHolder[] allWeapons = GetComponentsInChildren<WeaponHolder>(true);

        Debug.Log($"HOLDERS FOUND: {allWeapons.Length}");

        foreach (WeaponHolder holder in allWeapons)
        {
            Debug.Log($"HOLDER FOUND: {holder.name}");

            if (holder.weaponBase == null)
            {
                Debug.LogError($"WEAPON BASE NULL ON: {holder.name}");
                continue;
            }

            Debug.Log($"WEAPON BASE ACTIVE BEFORE START: {holder.weaponBase.gameObject.activeSelf}");
        }
    }

    private void Start()
    {
        Debug.Log("PLAYER INVENTORY START");

        WeaponHolder[] allWeapons = GetComponentsInChildren<WeaponHolder>(true);

        foreach (WeaponHolder holder in allWeapons)
        {
            Debug.Log($"DISABLING HOLDER: {holder.name}");

            holder.gameObject.SetActive(false);

            if (holder.weaponBase != null)
            {
                Debug.Log($"WEAPON BASE AFTER HOLDER DISABLE: {holder.weaponBase.name} ACTIVE = {holder.weaponBase.gameObject.activeSelf}");
            }
        }

        if (ownedWeapons.Count > 0)
        {
            Debug.Log($"ACTIVATING START WEAPON: {ownedWeapons[0].name}");

            ownedWeapons[0].gameObject.SetActive(true);

            Debug.Log($"START WEAPON ACTIVE: {ownedWeapons[0].gameObject.activeSelf}");

            if (ownedWeapons[0].weaponBase != null)
            {
                Debug.Log($"START WEAPON BASE ACTIVE: {ownedWeapons[0].weaponBase.gameObject.activeSelf}");
            }

            EquipWeapon(0);
        }
    }

    private void Update()
    {
        if (input == null)
        {
            Debug.LogError("INPUT NULL");
            return;
        }

        HandleScroll();
    }

    private void HandleScroll()
    {
        if (input.ScrollUpTriggered)
        {
            Debug.Log("SCROLL UP");

            NextWeapon();
        }

        if (input.ScrollDownTriggered)
        {
            Debug.Log("SCROLL DOWN");

            PreviousWeapon();
        }
    }

    public void AddWeapon(WeaponType type)
    {
        Debug.Log($"TRYING TO ADD WEAPON: {type}");

        foreach (WeaponHolder owned in ownedWeapons)
        {
            Debug.Log($"OWNED: {owned.weaponType}");

            if (owned.weaponType == type)
            {
                Debug.Log($"ALREADY OWNED: {type}");
                return;
            }
        }

        WeaponHolder[] allWeapons = GetComponentsInChildren<WeaponHolder>(true);

        Debug.Log($"TOTAL HOLDERS IN PLAYER: {allWeapons.Length}");

        foreach (WeaponHolder holder in allWeapons)
        {
            Debug.Log($"CHECKING HOLDER: {holder.name}");

            Debug.Log($"HOLDER TYPE: {holder.weaponType}");

            Debug.Log($"HOLDER ACTIVE: {holder.gameObject.activeSelf}");

            if (holder.weaponBase != null)
            {
                Debug.Log($"WEAPON BASE: {holder.weaponBase.name}");

                Debug.Log($"WEAPON BASE ACTIVE: {holder.weaponBase.gameObject.activeSelf}");
            }
            else
            {
                Debug.LogError($"WEAPON BASE NULL ON HOLDER: {holder.name}");
            }

            if (holder.weaponType != type)
                continue;

            Debug.Log($"MATCH FOUND: {holder.name}");

            ownedWeapons.Add(holder);

            holder.gameObject.SetActive(true);

            Debug.Log($"HOLDER ACTIVE AFTER ENABLE: {holder.gameObject.activeSelf}");

            if (holder.weaponBase != null)
            {
                Debug.Log($"WEAPON BASE ACTIVE AFTER ENABLE: {holder.weaponBase.gameObject.activeSelf}");
            }

            EquipWeapon(ownedWeapons.Count - 1);

            Debug.Log($"WEAPON ADDED: {type}");

            return;
        }

        Debug.LogError($"NO WEAPON HOLDER FOUND: {type}");
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
        Debug.Log($"EQUIP INDEX: {index}");

        if (ownedWeapons.Count <= 0)
        {
            Debug.LogError("NO OWNED WEAPONS");
            return;
        }

        currentIndex = index;

        for (int i = 0; i < ownedWeapons.Count; i++)
        {
            if (ownedWeapons[i] == null)
            {
                Debug.LogError($"NULL WEAPON AT INDEX: {i}");
                continue;
            }

            bool active = i == index;

            Debug.Log($"SETTING {ownedWeapons[i].name} ACTIVE = {active}");

            ownedWeapons[i].gameObject.SetActive(active);

            Debug.Log($"RESULT ACTIVE: {ownedWeapons[i].gameObject.activeSelf}");

            if (ownedWeapons[i].weaponBase != null)
            {
                Debug.Log($"WEAPON BASE ACTIVE: {ownedWeapons[i].weaponBase.gameObject.activeSelf}");
            }
        }

        CurrentWeapon = ownedWeapons[index];

        if (CurrentWeapon == null)
        {
            Debug.LogError("CURRENT WEAPON NULL");
            return;
        }

        Debug.Log($"CURRENT WEAPON: {CurrentWeapon.name}");

        UpdateAnimator(CurrentWeapon.weaponType);

        Debug.Log($"EQUIPPED: {CurrentWeapon.weaponType}");
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

        Debug.Log($"ANIMATOR UPDATED: {type}");
    }
}