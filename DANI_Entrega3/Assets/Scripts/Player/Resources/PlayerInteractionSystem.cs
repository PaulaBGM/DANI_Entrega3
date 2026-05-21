using UnityEngine;

public class PlayerInteractionSystem :
    MonoBehaviour
{
    [SerializeField]
    private PlayerInputHandler input;

    private IInteractable currentInteractable;

    private bool interactionConsumed;

    private void Awake()
    {
        if (input == null)
        {
            input =
                GetComponent<PlayerInputHandler>();
        }
    }

    private void Update()
    {
        if (currentInteractable == null)
            return;

        if (input.InteractPressed &&
            !interactionConsumed)
        {
            interactionConsumed = true;

            currentInteractable.Interact(
                gameObject);

            currentInteractable
                .OnInteractableDeactivated();

            currentInteractable = null;
        }

        if (!input.InteractPressed)
        {
            interactionConsumed = false;
        }
    }

    private void OnTriggerEnter(
        Collider other)
    {
        IInteractable interactable =
            other.GetComponent<IInteractable>();

        if (interactable == null)
            return;

        if (interactable != currentInteractable)
        {
            currentInteractable
                ?.OnInteractableDeactivated();
        }

        currentInteractable =
            interactable;

        currentInteractable
            ?.OnInteractableActivated();

        Debug.Log(
            $"INTERACTABLE FOUND: {other.name}");
    }

    private void OnTriggerExit(
        Collider other)
    {
        IInteractable interactable =
            other.GetComponent<IInteractable>();

        if (interactable == null)
            return;

        interactable
            .OnInteractableDeactivated();

        if (interactable ==
            currentInteractable)
        {
            currentInteractable = null;
        }

        Debug.Log(
            $"INTERACTABLE EXIT: {other.name}");
    }
}