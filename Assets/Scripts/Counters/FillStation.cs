using System;
using Unity.VisualScripting;
using UnityEngine;

public class FillStation : BaseCounter
{
    public event EventHandler <OnProgressChangedEventArgs> OnProgressChanged;
    public class OnProgressChangedEventArgs : EventArgs
    {
        public float progressNormalized;
    }
    [SerializeField] private FilledItemSO[] filledItemSOArray;


    private int filledProgress; 
   
    public override void Interact(PlayerController playerController)
    {
        //No item on counter
        if (!HasItemObject())
        {
            if (playerController.HasItemObject())
            {
                //Player can place down item on this counter
                if (HasFillWithInput(playerController.GetItemObject().GetItemObjectSO()))
                {
                    // Player holding item that can be filled
                    playerController.GetItemObject().SetItemObjectParent(this);
                    filledProgress = 0;

                    FilledItemSO filledItemSO = GetFilledItemWithInput(GetItemObject().GetItemObjectSO());

                    
                    OnProgressChanged?.Invoke(this, new OnProgressChangedEventArgs
                    {
                        progressNormalized = (float)filledProgress/ filledItemSO.fillProgressMax
                    });
                }
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

    public override void InteractAlternate(PlayerController playerController)
    {
        if (HasItemObject() && HasFillWithInput(GetItemObject().GetItemObjectSO()))
        {
            //There is an item on here AND can be filled
            filledProgress++;
            
            FilledItemSO filledItemSO = GetFilledItemWithInput(GetItemObject().GetItemObjectSO());

            OnProgressChanged?.Invoke(this, new OnProgressChangedEventArgs
            {
                progressNormalized = (float)filledProgress/ filledItemSO.fillProgressMax
            });

            if(filledProgress >= filledItemSO.fillProgressMax){
                ItemSO outputItemSO = GetOutputForInput(GetItemObject().GetItemObjectSO());
                GetItemObject().DestroySelf();

                ItemObject.SpawnItemObject(outputItemSO, this);
            }
        }
    }

    private bool HasFillWithInput(ItemSO inputItemObjectSO)
    {
        FilledItemSO filledItemSO = GetFilledItemWithInput(inputItemObjectSO);
        return filledItemSO != null;
    }

    private ItemSO GetOutputForInput(ItemSO inputItemObjectSO)
    {
        FilledItemSO filledItemSO = GetFilledItemWithInput(inputItemObjectSO);
        if(filledItemSO != null)
        {
            return filledItemSO.output;
        }
        else
        {
            return null;
        }
    }

    private FilledItemSO GetFilledItemWithInput(ItemSO inputItemObjectSO)
    {
        foreach(FilledItemSO filledItemSO in filledItemSOArray)
        {
            if(filledItemSO.input == inputItemObjectSO)
            {
                return filledItemSO;
            }
        }
        return null;
    }
}
