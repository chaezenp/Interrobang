using System;
using UnityEngine;

public class RiceCookerCounter : BaseCounter, IHasProgress
{    
    private enum State
    {
        Idle,
        Cooking,
        Cooked,
    }

    public event EventHandler <IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;

    [SerializeField] private Transform CounterTopPoint2;
    [SerializeField] private RiceCookerItemSO[] riceCookerItemSOArray;
    [SerializeField] private BurnedItemSO[] burnedItemSOArray;
    
    private State state;
    private float cookingTimer;
    private RiceCookerItemSO riceCookerItemSO;


    private void Start()
    {
        state = State.Idle;
    }
    private void Update()
    {
        if (HasItemObject())
        {
        switch (state)
        {
            case State.Idle:
                break;
            case State.Cooking:
                cookingTimer += Time.deltaTime;

                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                {
                    progressNormalized = cookingTimer / riceCookerItemSO.cookProgressMax
                });

                if(cookingTimer > riceCookerItemSO.cookProgressMax)
                {
                    GetItemObject().DestroySelf();
                    counterTopPoint = CounterTopPoint2;
                    ItemObject.SpawnItemObject(riceCookerItemSO.output, this);

                    state = State.Cooked;
                }
                break;
            case State.Cooked:

                break;

        }
    }
        
    }
    public override void Interact(PlayerController playerController)
    {
        //No item on counter
        if (!HasItemObject())
        {
            if (playerController.HasItemObject())
            {
                //Player can place down item on this counter
                if (HasCookWithInput(playerController.GetItemObject().GetItemObjectSO()))
                {
                    // Player holding item that can be cooked
                    playerController.GetItemObject().SetItemObjectParent(this);

                    riceCookerItemSO = GetCookedItemWithInput(GetItemObject().GetItemObjectSO());

                    state = State.Cooking;
                    cookingTimer = 0f;

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = cookingTimer / riceCookerItemSO.cookProgressMax
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
                if(playerController.GetItemObject() is BowlItemObject)
                {
                    //Player is holding a bowl
                    BowlItemObject bowlItemObject = playerController.GetItemObject() as BowlItemObject;
                    if(bowlItemObject.TryAddIngredient(GetItemObject().GetItemObjectSO()))
                    {
                    GetItemObject().DestroySelf();
                    
                    state = State.Idle;

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = 0f
                    });
                    }
                }
            }
            else
            {
                // Player cannot pick up raw
                //GetItemObject().SetItemObjectParent(playerController);

                
            }
        } 
    }

    private bool HasCookWithInput(ItemSO inputItemObjectSO)
    {
        RiceCookerItemSO riceCookerItemSO = GetCookedItemWithInput(inputItemObjectSO);
        return riceCookerItemSO != null;
    }

    private ItemSO GetOutputForInput(ItemSO inputItemObjectSO)
    {
        RiceCookerItemSO riceCookerItemSO = GetCookedItemWithInput(inputItemObjectSO);
        if(riceCookerItemSO != null)
        {
            return riceCookerItemSO.output;
        }
        else
        {
            return null;
        }
    }

    private RiceCookerItemSO GetCookedItemWithInput(ItemSO inputItemObjectSO)
    {
        foreach(RiceCookerItemSO riceCookerItemSO in riceCookerItemSOArray)
        {
            if(riceCookerItemSO.input == inputItemObjectSO)
            {
                return riceCookerItemSO;
            }
        }
        return null;
    }

    private BurnedItemSO GetBurningItemWithInput(ItemSO inputItemObjectSO)
    {
        foreach(BurnedItemSO burnedItemSO in burnedItemSOArray)
        {
            if(burnedItemSO.input == inputItemObjectSO)
            {
                return burnedItemSO;
            }
        }
        return null;
    }

}
