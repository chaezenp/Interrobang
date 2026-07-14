using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class PlayerInputController : MonoBehaviour
{
    public event EventHandler OnInteractAction;
    public event EventHandler OnTestInteractAction;
    public event EventHandler OnAltButtonAction;
    public event EventHandler OnInteractHoldAction;
    public event EventHandler OnInteractHoldCanceled;


    public Vector2 MovementInputVector { get; private set; }

    private InputSystem_Actions playerInputActions;

    private void Awake()
    {
        playerInputActions = new InputSystem_Actions();
        playerInputActions.Player.Enable();

        playerInputActions.Player.Interact.performed += OnInteractButtonPressed;
        playerInputActions.Player.Interact.canceled += OnInteractCanceled; 
        playerInputActions.Player.TestInteract.performed += OnTestButtonPressed;
        playerInputActions.Player.AltInteract.performed += OnAltButtonPressed;
    }

    private void OnInteractCanceled(InputAction.CallbackContext obj)
    {
        if (obj.interaction is HoldInteraction)
        {
            OnInteractHoldCanceled?.Invoke(this, EventArgs.Empty);
        }
    }

    private void OnAltButtonPressed(InputAction.CallbackContext obj)
    {
        OnAltButtonAction?.Invoke(this, EventArgs.Empty);
    }

    private void OnInteractButtonPressed(InputAction.CallbackContext obj)
    {
        //Debug.Log(obj);
        if (obj.interaction is TapInteraction)
        {
            OnInteractAction?.Invoke(this, EventArgs.Empty); 
        }
        else if (obj.interaction is HoldInteraction)
        {
            OnInteractHoldAction?.Invoke(this, EventArgs.Empty); 
        }
    
    }

    private void OnTestButtonPressed(InputAction.CallbackContext obj)
    {
        OnTestInteractAction?.Invoke(this, EventArgs.Empty);
    }

    private void OnMove(InputValue inputValue)
    {
        MovementInputVector = inputValue.Get<Vector2>();
    }
    public Vector2 GetMovementVectorNormalized()
    {
        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();
        inputVector = inputVector.normalized;
        return inputVector;
    }

}
