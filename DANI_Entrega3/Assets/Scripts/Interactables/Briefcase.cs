using UnityEngine;

public class Briefcase : Interactables, IInteractable
{
    public override void Interact(GameObject interactor)
    {
        Debug.Log("BRIEFCASE INTERACTED");

        PlayerMain main =
            interactor.GetComponent<PlayerMain>();

        if (main == null)
        {
            main =
                interactor.GetComponentInParent<PlayerMain>();
        }

        if (main == null)
        {
            Debug.LogError(
                "PLAYER MAIN NOT FOUND");

            return;
        }

        Debug.Log(
            "PLAYER MAIN FOUND");

        main.PlayerNotifiesBriefCase();
    }
}