using System.Runtime.Serialization;
using UnityEngine;

public class ClearCounter : BaseCounter
{
    //Real ClearCounter Script

    public override void Interact(PlayerController playerController)
    {
        //No item on counter
        if (!HasItemObject())
        {
            if (playerController.HasItemObject())
            {
                //Player can place down item on this counter
                playerController.GetItemObject().SetItemObjectParent(this);
            }
            else
            {
                //Player not holding anything

            }
        }
        else
        {
            //Theres an item on counter
            if (playerController.HasItemObject())
            {
                //Player is carrying something
            }
            else
            {
                // Player not carrying aything so can pick up
                GetItemObject().SetItemObjectParent(playerController);
            }
        } 
    }

}
