using UnityEngine;

public interface IInteractable
{
    void OnInteractableActivated();
    void OnInteractableDeactivated();
    void Interact(GameObject interactor);
}
