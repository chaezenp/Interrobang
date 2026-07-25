using UnityEngine;

public class ItemObject : MonoBehaviour
{
    [SerializeField] private ItemSO itemSO;

    private IItemObjectParent itemObjectParent;

    public ItemSO GetItemObjectSO()
    {
        return itemSO;
    }

    public void SetItemObjectParent(IItemObjectParent itemObjectParent)
    {
        if (this.itemObjectParent != null)
        {
            this.itemObjectParent.ClearItemObject();
        }

        this.itemObjectParent = itemObjectParent;
        if (itemObjectParent.HasItemObject())
        {
            Debug.LogError("IItemObjectParent already has object");
        }
        itemObjectParent.SetItemObject(this);

        transform.parent = itemObjectParent.GetItemObjectFollowTransform();
        transform.localPosition = Vector3.zero;
    }

    public IItemObjectParent GetItemObjectParent()
    {
        return itemObjectParent;
    }

    public void DestroySelf()
    {
        itemObjectParent.ClearItemObject();
        Destroy(gameObject);
    }

    public static ItemObject SpawnItemObject(ItemSO itemObjectSO, IItemObjectParent itemObjectParent)
    {
        Transform itemObjectTransform = Instantiate(itemObjectSO.prefab);
        
        ItemObject itemObject = itemObjectTransform.GetComponent<ItemObject>();
        
        itemObject.SetItemObjectParent(itemObjectParent);
        
        return itemObject;
    }
    public void SetItemSO(ItemSO newItemSO)
    {
        this.itemSO = newItemSO;
    }
}
