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
            GetComponent<PlayerInventory>();
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (inventory.CurrentWeapon == null)
            return;

        WeaponBase weapon =
            inventory.CurrentWeapon.weaponBase;

        if (weapon == null)
            return;

        if (input.ShootPressed &&
            timer >= weapon.data.fireRate)
        {
            timer = 0f;

            weapon.Fire();
        }
    }
}