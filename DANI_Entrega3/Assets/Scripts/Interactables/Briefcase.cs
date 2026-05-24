using UnityEngine;

public class Briefcase : Interactables, IInteractable
{
    public override void Interact(GameObject interactor)
    {
        var main = interactor.GetComponentInParent<PlayerMain>();
        
        main.PlayerNotifiesBriefCase();
    }
}
