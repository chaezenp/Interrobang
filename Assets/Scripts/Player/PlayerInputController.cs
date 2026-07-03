using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    public event EventHandler OnInteractAction, OnTestInteractAction;
    public Vector2 MovementInputVector { get; private set; }

    private InputSystem_Actions playerInputActions;

    private void Awake()
    {
        playerInputActions = new InputSystem_Actions();
        playerInputActions.Player.Enable();

        playerInputActions.Player.Interact.performed += OnInteractButtonPressed;
        playerInputActions.Player.TestInteract.performed += OnTestButtonPressed;
    }

    private void OnInteractButtonPressed(InputAction.CallbackContext obj)
    {
        //Debug.Log(obj);
        OnInteractAction?.Invoke(this, EventArgs.Empty);
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
