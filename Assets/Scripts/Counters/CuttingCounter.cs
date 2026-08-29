using System;
using Unity.VisualScripting;
using UnityEngine;

//Prev fillStation but now cutting as cut fish or other mash button prep
public class CuttingCounter : BaseCounter, IHasProgress
{
    public event EventHandler <IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    public event EventHandler OnItemPlaced;
    [SerializeField] private CuttingItemSO[] cutItemSOArray;
    [SerializeField] private ChopInputUI inputUI;


    private int cuttingProgress; 
   
    public override void Interact(PlayerController playerController)
    {
        //No item on counter
        if (!HasItemObject())
        {
            if (playerController.HasItemObject())
            {
                //Player can place down item on this counter
                if (HasCutWithInput(playerController.GetItemObject().GetItemObjectSO()))
                {
                    // Player holding item that can be filled
                    playerController.GetItemObject().SetItemObjectParent(this);
                    cuttingProgress = 0;
                    
                    CuttingItemSO cuttingItemSO = GetCutItemWithInput(GetItemObject().GetItemObjectSO());

                    
                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = (float)cuttingProgress/ cuttingItemSO.fillProgressMax
                    });
                    OnItemPlaced?.Invoke(this, EventArgs.Empty);

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
                if(playerController.GetItemObject() is BowlItemObject)
                {
                    //Player is holding a bowl
                    BowlItemObject bowlItemObject = playerController.GetItemObject() as BowlItemObject;
                    if(bowlItemObject.TryAddIngredient(GetItemObject().GetItemObjectSO()))
                    {
                    GetItemObject().DestroySelf();
                    
                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = 0f
                    });

                    if (inputUI != null)
                    {
                        inputUI.ResetBools();
                    }
                    }
                }
            }
            else
            {
                // Player not carrying aything so can pick up
                //GetItemObject().SetItemObjectParent(playerController);
            }
        } 
    }

    public override void InteractAlternate(PlayerController playerController)
    {
        if (HasItemObject() && HasCutWithInput(GetItemObject().GetItemObjectSO()))
        {
            //There is an item on here AND can be filled
            cuttingProgress++;
            
            CuttingItemSO cuttingItemSO = GetCutItemWithInput(GetItemObject().GetItemObjectSO());

            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
            {
                progressNormalized = (float)cuttingProgress/ cuttingItemSO.fillProgressMax
            });

            if (inputUI != null)
            {
                inputUI.ChangeTapNumber(cuttingProgress);
            }

            if(cuttingProgress >= cuttingItemSO.fillProgressMax){
                ItemSO outputItemSO = GetOutputForInput(GetItemObject().GetItemObjectSO());
                GetItemObject().DestroySelf();

                ItemObject.SpawnItemObject(outputItemSO, this);
            }
        }
    }

    private bool HasCutWithInput(ItemSO inputItemObjectSO)
    {
        CuttingItemSO cuttingItemSO = GetCutItemWithInput(inputItemObjectSO);
        return cuttingItemSO != null;
    }

    private ItemSO GetOutputForInput(ItemSO inputItemObjectSO)
    {
        CuttingItemSO cuttingItemSO = GetCutItemWithInput(inputItemObjectSO);
        if(cuttingItemSO != null)
        {
            return cuttingItemSO.output;
        }
        else
        {
            return null;
        }
    }

    private CuttingItemSO GetCutItemWithInput(ItemSO inputItemObjectSO)
    {
        foreach(CuttingItemSO cuttingItemSO in cutItemSOArray)
        {
            if(cuttingItemSO.input == inputItemObjectSO)
            {
                return cuttingItemSO;
            }
        }
        return null;
    }
}
