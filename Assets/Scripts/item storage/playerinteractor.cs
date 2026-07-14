using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    public Camera playerCamera;
    public InventoryManager inventory;

    public float interactDistance = 3f;
    public LayerMask interactLayer;


    public void Interact(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;


        Ray ray = new Ray(
            playerCamera.transform.position,
            playerCamera.transform.forward
        );


        if(Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactLayer))
        {
            PickupItem item = hit.collider.GetComponent<PickupItem>();

            if(item != null)
            {
                bool pickedUp = inventory.AddItem(item.itemData);

                if(pickedUp)
                {
                    Destroy(item.gameObject);
                }
            }
        }
    }
}
