using UnityEngine;

public class TrashCounter : BaseCounter
{

    public override void Interact(PlayerController playerController)
    {
        if (playerController.HasItemObject())
        {
            playerController.GetItemObject().DestroySelf();
        }
    }

}
