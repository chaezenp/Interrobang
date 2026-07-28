using System;
using UnityEngine;

public class FillStationCounter : BaseCounter, IHasProgress
{    
    private enum State
    {
        Idle,
        Filling,
        Filled,
    }

    public event EventHandler <IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;

    [SerializeField] private FillingItemSO[] fillingItemSOArray;
    
    private State state;
    private float fillingTimer;
    private FillingItemSO fillingItemSO;


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
            case State.Filling:
                fillingTimer += Time.deltaTime;

                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                {
                    progressNormalized = fillingTimer / fillingItemSO.fillProgressMax
                });

                if(fillingTimer > fillingItemSO.fillProgressMax)
                {
                    GetItemObject().DestroySelf();
                    ItemObject.SpawnItemObject(fillingItemSO.output, this);

                    state = State.Filled;
                }
                break;
            case State.Filled:


                    state = State.Idle;

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = 0f
                    });
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
                if (HasFillWithInput(playerController.GetItemObject().GetItemObjectSO()))
                {
                    // Player holding item that can be cooked
                    playerController.GetItemObject().SetItemObjectParent(this);

                    fillingItemSO = GetFilledItemWithInput(GetItemObject().GetItemObjectSO());

                    state = State.Filling;
                    fillingTimer = 0f;

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = fillingTimer / fillingItemSO.fillProgressMax
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

                state = State.Idle;

                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                {
                    progressNormalized = 0f
                });
            }
        } 
    }

    private bool HasFillWithInput(ItemSO inputItemObjectSO)
    {
        FillingItemSO fillingItemSO = GetFilledItemWithInput(inputItemObjectSO);
        return fillingItemSO != null;
    }

    private ItemSO GetOutputForInput(ItemSO inputItemObjectSO)
    {
        FillingItemSO fillingItemSO = GetFilledItemWithInput(inputItemObjectSO);
        if(fillingItemSO != null)
        {
            return fillingItemSO.output;
        }
        else
        {
            return null;
        }
    }

    private FillingItemSO GetFilledItemWithInput(ItemSO inputItemObjectSO)
    {
        foreach(FillingItemSO fillingItemSO in fillingItemSOArray)
        {
            if(fillingItemSO.input == inputItemObjectSO)
            {
                return fillingItemSO;
            }
        }
        return null;
    }

}
