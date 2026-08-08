using UnityEngine;

public class BaseCounter : MonoBehaviour, IItemObjectParent
{

    public Transform counterTopPoint;

    private ItemObject itemObject;

    public virtual void Interact(PlayerController playerController)
    {
        Debug.LogError("BaseCounter.Interact();");        
    }

    public virtual void InteractAlternate(PlayerController playerController)
    {
        //Debug.LogError("BaseCounter.InteractAlternate()");
    }

    public virtual void InteractHold(PlayerController playerController)
    {
        //Debug.LogError("BaseCounter.InteractAlternateHold()");
    }
    public virtual void InteractHoldRelease(PlayerController playerController)
    {
        //Debug.LogWarning("BaseCounter.InteractAlternateRelease()");
    }

    public Transform GetItemObjectFollowTransform()
    {
        return counterTopPoint;
    }

    public void SetItemObject(ItemObject itemObject)
    {
        this.itemObject = itemObject;
    }

    public ItemObject GetItemObject()
    {
        return itemObject;
    }

    public void ClearItemObject()
    {
        itemObject = null;
    }

    public bool HasItemObject()
    {
        return itemObject != null;
    }
}
