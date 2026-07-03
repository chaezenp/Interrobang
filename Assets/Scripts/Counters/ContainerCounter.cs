using UnityEngine;

public class ContainerCounter : BaseCounter
{
    //Used to get an item out from a container, cant place it on here
    [SerializeField] private ItemSO ItemSO;

        public override void Interact(PlayerController playerController)
    {   //give obj to player
        if (!playerController.HasItemObject())
        {
            //Player not carrying anything
        Transform itemObjectTransform = Instantiate(ItemSO.prefab);
        itemObjectTransform.GetComponent<ItemObject>().SetItemObjectParent(playerController);
        } //4:10:00 in video for animation
    }

}
