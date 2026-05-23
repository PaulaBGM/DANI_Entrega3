using System.Collections;
using UnityEngine;

public class PlayerWeaponPickup : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    [SerializeField]
    private float pickupRadius = 2f;

    [SerializeField]
    private LayerMask weaponLayer;

    [SerializeField]
    private Transform pickupPoint;

    [SerializeField]
    private float pickupDelay = 0.8f;

    private PlayerInputHandler input;

    private PlayerInventory inventory;

    private bool isPickingUp;

    private static readonly int PickUp =
        Animator.StringToHash("pickUp");

    private void Awake()
    {
        input = GetComponent<PlayerInputHandler>();

        inventory = GetComponent<PlayerInventory>();

        if (inventory == null)
        {
            inventory =
                GetComponentInChildren<PlayerInventory>();
        }

        if (animator == null)
        {
            animator =
                GetComponentInParent<Animator>();
        }

        Debug.Log(
            $"INVENTORY FOUND: {inventory}");
    }

    private void Update()
    {
        if (isPickingUp)
            return;

        if (!input.InteractTriggered)
            return;

        Collider[] hits =
            Physics.OverlapSphere(
                pickupPoint.position,
                pickupRadius,
                weaponLayer);

        if (hits.Length <= 0)
            return;

        Weapon closestWeapon = null;

        float closestDistance =
            Mathf.Infinity;

        foreach (Collider hit in hits)
        {
            Weapon weapon =
                hit.GetComponentInParent<Weapon>();

            if (weapon == null)
                continue;

            float distance =
                Vector3.Distance(
                    transform.position,
                    weapon.transform.position);

            if (distance < closestDistance)
            {
                closestDistance =
                    distance;

                closestWeapon =
                    weapon;
            }
        }

        if (closestWeapon == null)
            return;

        StartCoroutine(
            PickupRoutine(
                closestWeapon));
    }

    private IEnumerator PickupRoutine(
        Weapon weapon)
    {
        isPickingUp = true;

        Debug.Log(
            $"PICKING UP: {weapon.weaponType}");

        animator.ResetTrigger(PickUp);

        animator.SetTrigger(PickUp);

        yield return new WaitForSeconds(
            pickupDelay);

        //inventory.AddWeapon(
            //weapon.weaponType);

        Destroy(
            weapon.gameObject);

        isPickingUp = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (pickupPoint == null)
            return;

        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(
            pickupPoint.position,
            pickupRadius);
    }
}