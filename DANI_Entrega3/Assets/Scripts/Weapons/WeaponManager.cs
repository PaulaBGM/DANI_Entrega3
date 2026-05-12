using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [Header("Weapons")]
    [SerializeField]
    private List<Weapon> weapons =
        new List<Weapon>();

    private Weapon currentWeapon;

    private int currentWeaponIndex;

    private float nextShootTime;

    public WeaponType CurrentWeaponType =>
        currentWeapon.weaponType;

    public bool IsHeavyWeaponEquipped =>
        currentWeapon.weaponType == WeaponType.Rifle ||
        currentWeapon.weaponType == WeaponType.GrenadeLauncher;

    private void Start()
    {
        InputController.Instance.OnShootPressed += StartShooting;
        InputController.Instance.OnScrollWeapon += ChangeWeaponScroll;
        InputController.Instance.OnSelectWeapon += SelectWeapon;

        EquipWeapon(0);
    }

    private bool isShooting;

    private void Update()
    {
        if (isShooting)
        {
            HandleShooting();
        }
    }

    private void StartShooting()
    {
        isShooting = true;
    }

    private void StopShooting()
    {
        isShooting = false;
    }

    private void HandleShooting()
    {
        if (Time.time < nextShootTime)
            return;

        if (!currentWeapon.HasAmmo())
        {
            SwitchToKnife();
            return;
        }

        currentWeapon.OnUse();

        nextShootTime =
            Time.time + currentWeapon.fireRate;
    }

    private void ChangeWeaponScroll(float direction)
    {
        if (direction > 0)
        {
            NextWeapon();
        }
        else if (direction < 0)
        {
            PreviousWeapon();
        }
    }

    private void SelectWeapon(int index)
    {
        if (index < 0 || index >= weapons.Count)
            return;

        EquipWeapon(index);
    }

    private void NextWeapon()
    {
        currentWeaponIndex++;

        if (currentWeaponIndex >= weapons.Count)
        {
            currentWeaponIndex = 0;
        }

        EquipWeapon(currentWeaponIndex);
    }

    private void PreviousWeapon()
    {
        currentWeaponIndex--;

        if (currentWeaponIndex < 0)
        {
            currentWeaponIndex =
                weapons.Count - 1;
        }

        EquipWeapon(currentWeaponIndex);
    }

    private void EquipWeapon(int index)
    {
        foreach (Weapon weapon in weapons)
        {
            weapon.gameObject.SetActive(false);
        }

        currentWeapon = weapons[index];

        currentWeaponIndex = index;

        currentWeapon.gameObject.SetActive(true);
    }

    private void SwitchToKnife()
    {
        for (int i = 0; i < weapons.Count; i++)
        {
            if (weapons[i].weaponType ==
                WeaponType.Knife)
            {
                EquipWeapon(i);
                return;
            }
        }
    }

    private void OnDisable()
    {
        if (InputController.Instance == null)
            return;

        InputController.Instance.OnShootPressed -= StartShooting;
        InputController.Instance.OnShootReleased -= StopShooting;

        InputController.Instance.OnScrollWeapon -= ChangeWeaponScroll;
        InputController.Instance.OnSelectWeapon -= SelectWeapon;
    }
    public bool HasWeapon(WeaponType type)
    {
        foreach (Weapon weapon in weapons)
        {
            if (weapon.weaponType == type)
            {
                return true;
            }
        }

        return false;
    }

}