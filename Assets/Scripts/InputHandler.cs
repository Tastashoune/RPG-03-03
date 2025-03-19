using UnityEngine;
using UnityEngine.InputSystem;


public abstract class InputHandler : MonoBehaviour
{
    protected virtual void Start()
    {
        // V�rifier que l'InputManager existe et a un PlayerInput valide
        if (InputManager.Instance != null && InputManager.Instance.CurrentPlayerInput != null)
        {
            // Enregistrer les actions
            RegisterInputActions();
        }
        else
        {
            Debug.LogError($"InputHandler in {gameObject.name} can't be Init: InputManager not find or PlayerInput null");


        }
    }

    protected virtual void OnEnable()
    {
        // Si d�j� d�marr�, on s'assure que les actions sont enregistr�es
        if (InputManager.Instance != null && InputManager.Instance.CurrentPlayerInput != null)
        {
            RegisterInputActions();
        }
    }

    protected virtual void OnDisable()
    {
        // Si l'InputManager existe toujours, on d�senregistre nos actions
        if (InputManager.Instance != null && InputManager.Instance.CurrentPlayerInput != null)
        {
            UnregisterInputActions();
        }
    }

    // Les m�thodes abstraites ne changent pas
    protected abstract void RegisterInputActions();
    protected abstract void UnregisterInputActions();

    // Helper pour avoir facilement acc�s au PlayerInput
    protected PlayerInput GetPlayerInput()
    {
        if (InputManager.Instance != null)
        {
            return InputManager.Instance.CurrentPlayerInput;
        }
        return null;
    }
}
