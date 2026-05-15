using UnityEngine;

public class AmmoBox : Interactables
{
    [SerializeField] private float ammoBoxAmount = 15f; 
    [SerializeField] private AudioClip reloadAmmoClip;
    public override void Interact(GameObject interactor)
    {
        var main = interactor.GetComponentInParent<PlayerMain>();
        if (main != null)
        {
            main.NotifyAmmoGunCollected(ammoBoxAmount);
        }
        
        AudioManager.Instance.PlaySFX(AudioManager.Instance.audioLibrary.ammoBoxSfx);
        gameObject.SetActive(false);
    }
}