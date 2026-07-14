using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [Header("Inventory")]

    public ItemData[] inventory = new ItemData[3];

    [Header("Hand")]

    public Transform handHolder;

    private GameObject currentHeldObject;

    private int currentSlot = -1;

    //------------------------------------------------------------

    public bool AddItem(ItemData item)
    {
        for(int i = 0; i < inventory.Length; i++)
        {
            if(inventory[i] == null)
            {
                inventory[i] = item;

                Debug.Log(item.itemName + " added to Slot " + (i + 1));

                return true;
            }
        }

        Debug.Log("Inventory Full");

        return false;
    }

    //------------------------------------------------------------

    public void EquipSlot(int slot)
    {
        if(slot < 0 || slot >= inventory.Length)
            return;

        currentSlot = slot;

        if(currentHeldObject != null)
            Destroy(currentHeldObject);

        if(inventory[slot] == null)
            return;

        currentHeldObject =
            Instantiate(
                inventory[slot].heldPrefab,
                handHolder);

        currentHeldObject.transform.localPosition = Vector3.zero;
        currentHeldObject.transform.localRotation = Quaternion.identity;

        Debug.Log("Equipped " + inventory[slot].itemName);
    }

    //------------------------------------------------------------

    public void DropCurrent()
    {
        if(currentSlot == -1)
            return;

        if(inventory[currentSlot] == null)
            return;

        if(currentHeldObject != null)
            Destroy(currentHeldObject);

        Debug.Log("Dropped " + inventory[currentSlot].itemName);

        inventory[currentSlot] = null;

        currentSlot = -1;
    }

    //------------------------------------------------------------

    public ItemData GetHeldItem()
    {
        if(currentSlot == -1)
            return null;

        return inventory[currentSlot];
    }
}