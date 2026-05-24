using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    private PlayerInputHandler input;

    private PlayerInventory inventory;

    private float timer;

    private void Awake()
    {
        input =
            GetComponent<PlayerInputHandler>();

        inventory =
            GetComponentInChildren<PlayerInventory>();
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (inventory == null)
        {
            return;
        }

        if (inventory.CurrentWeapon == null)
        {
            return;
        }
        
        if (inventory.CurrentWeapon.weaponBase == null)
        {
            return;
        }

        WeaponBase weapon = inventory.CurrentWeapon.weaponBase;
        
        if (weapon.data == null)
        {
            return;
        }

        if (input == null)
        {
            return;
        }

        if (input.ShootPressed &&
            timer >= weapon.data.fireRate)
        {
            timer = 0f;

            weapon.Fire();
        }
    }
}