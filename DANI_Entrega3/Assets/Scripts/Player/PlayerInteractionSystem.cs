using System;
using UnityEngine;

public class PlayerInteractionSystem : MonoBehaviour
{
    private IInteractable currentInteractable;
    
    private void OnEnable()
    {
        InputController.Instance.OnInteract += LaunchInteraction;
    }

    private void LaunchInteraction() //Interacción con el objeto actual
    {
        currentInteractable?.Interact(gameObject);
        currentInteractable?.OnInteractableDeactivated();
        currentInteractable = null;
    }

    private void OnTriggerEnter(Collider other) //Actualización del interactuador actual.
    {
        if (other.TryGetComponent(out IInteractable interactable))
        {
            if (interactable != currentInteractable)
            {
                currentInteractable?.OnInteractableDeactivated();
            }
            
            currentInteractable = interactable;
            currentInteractable?.OnInteractableActivated();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out IInteractable interactable)) 
        {
            interactable.OnInteractableDeactivated();

            if (interactable == currentInteractable)
            {
                currentInteractable = null;
            }
        }
    }
}
