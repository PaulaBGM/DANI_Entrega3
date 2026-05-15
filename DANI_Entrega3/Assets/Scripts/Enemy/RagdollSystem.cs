using System;
using UnityEngine;

public class RagdollSystem : MonoBehaviour
{
    private Rigidbody[] bones;

    //Buscamos en los hijos los rigidbodies y los pone en kinemático.
    private void Awake()
    {
        bones = GetComponentsInChildren<Rigidbody>();

        UpdateBonesState(true);
    }

    public void UpdateBonesState(bool state)
    {
        foreach (var bone in  bones)
        {
            bone.isKinematic = state;
        }
    }
    
    
}
