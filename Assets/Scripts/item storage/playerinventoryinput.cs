using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventoryInput : MonoBehaviour
{
    public InventoryManager inventory;

    public void Slot1(InputAction.CallbackContext context)
    {
        if(context.performed)
            inventory.EquipSlot(0);
    }

    public void Slot2(InputAction.CallbackContext context)
    {
        if(context.performed)
            inventory.EquipSlot(1);
    }

    public void Slot3(InputAction.CallbackContext context)
    {
        if(context.performed)
            inventory.EquipSlot(2);
    }

    public void Drop(InputAction.CallbackContext context)
    {
        if(context.performed)
            inventory.DropCurrent();
    }
}