using UnityEngine;

public class PlayerInteractionSystem :
    MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private PlayerInputHandler input;

    [Header("Settings")]
    [SerializeField]
    private float interactRadius = 2f;

    [SerializeField]
    private LayerMask interactableLayer;

    [SerializeField]
    private Transform interactionPoint;

    private IInteractable currentInteractable;

    private bool interactionConsumed;

    private void Awake()
    {
        if (input == null)
        {
            input =
                GetComponent<PlayerInputHandler>();
        }

        if (interactionPoint == null)
        {
            interactionPoint = transform;
        }
    }

    private void Update()
    {
        DetectInteractables();

        HandleInteraction();
    }

    // =========================
    // DETECT
    // =========================

    private void DetectInteractables()
    {
        Collider[] hits =
            Physics.OverlapSphere(
                interactionPoint.position,
                interactRadius,
                interactableLayer);

        if (hits.Length <= 0)
        {
            if (currentInteractable != null)
            {
                currentInteractable
                    .OnInteractableDeactivated();

                currentInteractable = null;
            }

            return;
        }

        IInteractable closest =
            null;

        float closestDistance =
            Mathf.Infinity;

        foreach (Collider hit in hits)
        {
            IInteractable interactable =
                hit.GetComponentInParent<IInteractable>();

            if (interactable == null)
                continue;

            float distance =
                Vector3.Distance(
                    transform.position,
                    hit.transform.position);

            if (distance < closestDistance)
            {
                closestDistance =
                    distance;

                closest =
                    interactable;
            }
        }

        if (closest == null)
            return;

        if (closest != currentInteractable)
        {
            currentInteractable
                ?.OnInteractableDeactivated();

            currentInteractable = closest;

            currentInteractable?.OnInteractableActivated();
        }
    }

    // =========================
    // INTERACT
    // =========================

    private void HandleInteraction()
    {
        if (currentInteractable == null)
            return;

        if (input.InteractPressed &&
            !interactionConsumed)
        {
            interactionConsumed = true;

            Debug.Log(
                $"INTERACTING WITH: {currentInteractable}");

            currentInteractable
                .Interact(gameObject);
        }

        if (!input.InteractPressed)
        {
            interactionConsumed = false;
        }
    }

    // =========================
    // GIZMOS
    // =========================

    private void OnDrawGizmosSelected()
    {
        if (interactionPoint == null)
            return;

        Gizmos.color =
            Color.green;

        Gizmos.DrawWireSphere(
            interactionPoint.position,
            interactRadius);
    }
}