using UnityEngine;

public class AmmoGrenadeBox : Interactables
{
    [SerializeField] private float ammoBoxAmount = 5f;

    public override void Interact(GameObject interactor)
    {
        var main = interactor.GetComponentInParent<PlayerMain>();
        if (main != null)
        {
            main.NotifyAmmoGrenadeCollected(ammoBoxAmount);
        }

        AudioManager.Instance.PlaySFX(AudioManager.Instance.audioLibrary.ammoBoxSfx);
        gameObject.SetActive(false);
    }
}