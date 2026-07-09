using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class DeliveryCounter : BaseCounter
{
    [SerializeField] private ValidItemRequestsSO validItemRequestsSO;
    [SerializeField] private ItemRequestSO currentItemRequest;

    public bool testingBool;
    private bool canRequest = false;
    private float SpawnItemTimer;
    private float SpawnItemTimerMax = 4f;

    //touristtimer ref
    private int buttonPressProgress;
    public override void Interact(PlayerController playerController)
    {
        if (playerController.HasItemObject())
            {                    
                //TODO: make sure that if walk away

                ItemSO itemSO = playerController.GetItemObject().GetItemObjectSO();

                if(currentItemRequest != null && currentItemRequest.itemSOList.Contains(itemSO))
                {
                    buttonPressProgress++;

                    if(buttonPressProgress >= itemSO.targetGoal)
                    {
                        currentItemRequest = null;
                        playerController.GetItemObject().DestroySelf();
                        Debug.Log("Item Delivered!");

                        buttonPressProgress = 0;
                        SpawnItemTimer = SpawnItemTimerMax;
                    }
                    
                    //For progress bar look at fill station
                    float inputProgress = (float)buttonPressProgress/ itemSO.targetGoal;

                }
                else
                {
                    // Wrong item, UI or sound or some indicator to tell you wrong Item
                    // Subtract time from timer for wrong item
                }                    
            }
            else
            {
                //Player not holding anything

            }
    }

    private void Update()
    {
        if (!currentItemRequest)
        {
            canRequest = true;
        }
        else {canRequest = false;}
        // Randomize getting items
        if (canRequest)
        {
            SpawnItemTimer -= Time.deltaTime;
            Debug.Log("Taro " + SpawnItemTimer);
            if (SpawnItemTimer <= 0f)
            {
                SpawnItemTimer = SpawnItemTimerMax;

                if (!currentItemRequest) {
                currentItemRequest = validItemRequestsSO.itemRequestSOList[Random.Range(0, validItemRequestsSO.itemRequestSOList.Count)];
                Debug.Log(currentItemRequest.requestName);
                }
            }
        }
    }
}
