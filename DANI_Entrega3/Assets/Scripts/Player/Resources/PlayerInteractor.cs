using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private float interactRange = 3f;

    private PlayerInputHandler input;

    private bool interactionConsumed;

    private void Awake()
    {
        input = GetComponent<PlayerInputHandler>();
    }

    private void Update()
    {
        if (input.InteractPressed && !interactionConsumed)
        {
            interactionConsumed = true;

            TryInteract();
        }

        if (!input.InteractPressed)
        {
            interactionConsumed = false;
        }
    }

    private void TryInteract()
    {
        Ray ray = new Ray(
            Camera.main.transform.position,
            Camera.main.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactRange))
        {
            if (hit.collider.TryGetComponent(out IInteractable interactable))
            {
                interactable.Interact(gameObject);
            }
        }
    }
}