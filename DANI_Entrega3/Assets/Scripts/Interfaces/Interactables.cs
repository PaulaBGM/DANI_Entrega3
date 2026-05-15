using UnityEngine;

public abstract class Interactables : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject interactionIcon;

    public void OnInteractableActivated()
    {
        interactionIcon.SetActive(true);
    }

    public void OnInteractableDeactivated()
    {
        interactionIcon.SetActive(false);
    }

    public abstract void Interact(GameObject interactor);
}
