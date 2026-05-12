using UnityEngine;

public abstract class States<T> : MonoBehaviour where T : FSMController<T> //Le indica a la clase que T debe heredar de FSMController<T>
{
    protected T _controller;

    protected PlayerHealthSystem playerHealth;

    public virtual void InitController(T controller) //Inicializa el controlador para cualquier estado.
    {
        this._controller = controller;
    }
    public abstract void OnEnter(); //Método a ejecutar al entrar en un estado.
    public abstract void OnUpdate(); //Método a realizar cuando se mantiene el estado.
    public abstract void OnExit(); //Método a ejecutar al salir del estado.
}
