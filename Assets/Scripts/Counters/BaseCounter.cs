using UnityEngine;

public class BaseCounter : MonoBehaviour, IItemObjectParent
{

    [SerializeField] private Transform counterTopPoint;

    private ItemObject itemObject;

    public virtual void Interact(PlayerController playerController)
    {
        Debug.LogError("BaseCounter.Interact();");        
    }

    public virtual void InteractAlternate(PlayerController playerController)
    {
        Debug.LogError("BaseCounter.InteractAlternate()");
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
