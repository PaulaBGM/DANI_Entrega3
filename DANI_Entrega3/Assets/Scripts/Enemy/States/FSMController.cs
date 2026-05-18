using System;
using UnityEngine;

public class FSMController<T> : MonoBehaviour where T : FSMController<T> //Le indica a la clase que T debe heredar de FSMController<T>
{
    private States<T> currentState;

    private void Update()
    {
        if (currentState)
        {
            currentState.OnUpdate();
        }
    }

    public void SetState(States<T> newState) //Comprueba si hay un estado anterior y lo actualiza.
    {
        if (currentState)
        {
            currentState.OnExit();
        }

        currentState = newState;
        currentState.OnEnter();
    }
}
