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
            Debug.LogError("INVENTORY NULL");
            return;
        }

        if (inventory.CurrentWeapon == null)
        {
            Debug.LogError("CURRENT WEAPON NULL");
            return;
        }

        Debug.Log(
            $"CURRENT HOLDER: {inventory.CurrentWeapon.name}");

        if (inventory.CurrentWeapon.weaponBase == null)
        {
            Debug.LogError(
                $"WEAPON BASE NULL ON HOLDER: {inventory.CurrentWeapon.name}");

            return;
        }

        WeaponBase weapon =
            inventory.CurrentWeapon.weaponBase;

        Debug.Log(
            $"WEAPON FOUND: {weapon.name}");

        if (weapon.data == null)
        {
            Debug.LogError(
                $"WEAPON DATA NULL: {weapon.name}");

            return;
        }

        if (input == null)
        {
            Debug.LogError("INPUT NULL");

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