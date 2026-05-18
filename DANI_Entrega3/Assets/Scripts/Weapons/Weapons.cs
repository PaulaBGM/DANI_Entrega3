using UnityEngine;

public abstract class Weapons : MonoBehaviour
{
    protected PlayerAmmoSystem playerAmmoSystem;

    public abstract void OnUse();
}
