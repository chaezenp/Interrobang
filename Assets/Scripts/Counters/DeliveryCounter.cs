using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.WSA;

public class DeliveryCounter : BaseCounter
{
    [SerializeField] private ValidItemRequestsSO validItemRequestsSO;
    // For multiple Requests need to turn this into a list or a new list variable 
    // Also need to add a new int to generate random requests to add to list
    // Look at deliver system in video
    [SerializeField] private ItemRequestSO currentItemRequest;
    [SerializeField] private PlayerController PC;
    [SerializeField] private TouristsTimer TT; 


    private bool canRequest = false;
    private float SpawnItemTimer;
    private float SpawnItemTimerMax = 4f;

    //For item list
    public int currentItemIndex = 0;
    private int buttonPressProgress;
    private float holdProgressTimer = 0f;
    private bool isPlayerHoldingButtonDown = false;
    private bool wrongItemPenaltyApplied = false;

    public override void Interact(PlayerController playerController)
    {

        if (playerController.HasItemObject())
            {        
                ItemSO itemSO = playerController.GetItemObject().GetItemObjectSO();
            
                //TODO: make sure that if walk away
                
                if(currentItemRequest != null && !currentItemRequest.itemSOList.Contains(itemSO))
                {
                    HandleWrongItemPenalty();
                    return;
                }

                if(itemSO.isHoldItem)
                {
                    isPlayerHoldingButtonDown = true;
                }
                else
                {
                    HandleTapLogic(itemSO);
                }
            }
            else
            {
                //Player not holding anything

            }
    }

    public override void InteractHold(PlayerController playerController)
    {
        isPlayerHoldingButtonDown = true;
    }

    public override void InteractHoldRelease(PlayerController playerController)
    {
        if(isPlayerHoldingButtonDown)
        {
            isPlayerHoldingButtonDown = false;
            wrongItemPenaltyApplied = false;
            ResetProgress();
        }
    }

    private void Awake()
    {
        if (!canRequest)
        {
            TT.requestActive = true;
        }
        else
        {
            TT.requestActive = false;
            TT.itemName = currentItemRequest.requestName;

        }
    }

    private void Update()
    {
        if(isPlayerHoldingButtonDown)
        {
            HandleHoldLogic();
        }

        if (!currentItemRequest)
        {
            canRequest = true;
        }
        else {canRequest = false;}
        // Randomize getting items
        if (canRequest)
        {   
            TT.requestActive = false;
            SpawnItemTimer -= Time.deltaTime;
            //Debug.Log("Taro " + SpawnItemTimer);
            if (SpawnItemTimer <= 0f)
            {
                SpawnItemTimer = SpawnItemTimerMax;

                if (!currentItemRequest) {
                currentItemRequest = validItemRequestsSO.itemRequestSOList[Random.Range(0, validItemRequestsSO.itemRequestSOList.Count)];
                //SpawnNewItem();
                Debug.Log(currentItemRequest.requestName);
                TT.itemName = currentItemRequest.requestName;
                }
            }
        }
        else
        {
            TT.requestActive = true;
        }
    }

    private void SpawnNewItem()
    { //Spawn items in list order
        // For testing new items
        if (validItemRequestsSO.itemRequestSOList == null || validItemRequestsSO.itemRequestSOList.Count == 0)
        {
            Debug.LogError("The itemRequestSOList is empty!");
            return;
        }

        currentItemRequest = validItemRequestsSO.itemRequestSOList[currentItemIndex];

        currentItemIndex++;

        if (currentItemIndex >= validItemRequestsSO.itemRequestSOList.Count)
        {
            currentItemIndex = 0; 
        }

    }

    private void HandleTapLogic(ItemSO itemSO)
    {
        buttonPressProgress++;
        
        if(buttonPressProgress >= itemSO.targetGoal)
        {
            CompleteDelivery();
        }
        //For progress bar look at fill station
        float inputProgress = (float)buttonPressProgress/ itemSO.targetGoal;

    }

    private void HandleHoldLogic()
    {
        if (!PC.HasItemObject())
        {
            InteractHoldRelease(PC);
            return;
        }

        ItemSO itemSO = PC.GetItemObject().GetItemObjectSO();
        
        holdProgressTimer += Time.deltaTime;
        if(holdProgressTimer >= itemSO.targetGoal)
        {
            CompleteDelivery();
        }
        
    }

    private void CompleteDelivery()
    {
        isPlayerHoldingButtonDown = false;
        currentItemRequest = null;
        TT.DeliverItem(true);
        PC.GetItemObject().DestroySelf();
        Debug.Log("Item Delivered!");

        ResetProgress();
    }

    private void HandleWrongItemPenalty()
    {
        if(!wrongItemPenaltyApplied)
        {
            Debug.Log("Wrong Item!");
            // Timer penalty 
            // Sound triggers
            // Visual red outline on timer & shake?
        }
    }

    private void ResetProgress()
    {
        buttonPressProgress = 0;
        SpawnItemTimer = SpawnItemTimerMax;
        holdProgressTimer = 0f;
        //UI
    }
}
