using UnityEngine;

public class WaterBottle : Interactables
{
    [SerializeField] private float hydratedAmount = 30f;
    public override void Interact(GameObject interactor)
    {
        var main = interactor.GetComponentInParent<PlayerMain>();
        if (main != null)
        {
            main.NotifiesHydrated(hydratedAmount);
        }

        AudioManager.Instance.PlaySFX(AudioManager.Instance.audioLibrary.drinkSfx);
        gameObject.SetActive(false);
    }
}
