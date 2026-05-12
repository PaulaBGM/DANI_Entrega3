using UnityEngine;

public class PlayerAnimEvent : MonoBehaviour
{
    public void WalkSound()
    {
        AudioManager.Instance.PlaySFX(AudioManager.Instance.audioLibrary.stepSfx);
    }
}
