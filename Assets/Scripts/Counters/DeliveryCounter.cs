using System;
using UnityEngine;

public class DeliveryCounter : BaseCounter
{
    [Header("Requests")]
    [SerializeField] private ValidItemRequestsSO validItemRequestsSO;
    [SerializeField] private ItemRequestSO currentItemRequest;
    [SerializeField] private TouristRequestUI requestUI;

    public PlayerController PC;

    // For presentations: cycle through the request list in order instead of randomly
    public bool presentTest;
    public int currentItemIndex = 0;


    [Header("Timer")]
    public int randTimerMin = 30;
    public int randTimerMax = 60;

    public event Action OnRequestFailed;

    public event Action OnRequestSucceeded;

    private TouristManager manager;

    private bool canRequest = false;
    private bool requestActive = false;
    private bool timerFinalized = false;
    private float timerValue;

    private float spawnItemTimer;
    private float spawnItemTimerMax = 4f;


    private int buttonPressProgress;
    private float holdProgressTimer = 0f;
    private bool isPlayerHoldingButtonDown = false;
    private bool wrongItemPenaltyApplied = false;

    public override void Interact(PlayerController playerController)
    {
        if (!playerController.HasItemObject())
        {
            // Player not holding anything.
            return;
        }

        ItemSO itemSO = playerController.GetItemObject().GetItemObjectSO();
        Debug.Log(itemSO);

        if (currentItemRequest != null && !currentItemRequest.itemSOList.Contains(itemSO))
        {
            HandleWrongItemPenalty();
            return;
        }

        if (itemSO.isHoldItem)
        {
            isPlayerHoldingButtonDown = true;
        }
        else
        {
            HandleTapLogic(itemSO);
        }
    }

    public void Initialize(TouristManager managerReference)
    {
        manager = managerReference;
    }

    public override void InteractHold(PlayerController playerController)
    {
        isPlayerHoldingButtonDown = true;
    }

    public override void InteractHoldRelease(PlayerController playerController)
    {
        if (isPlayerHoldingButtonDown)
        {
            isPlayerHoldingButtonDown = false;
            wrongItemPenaltyApplied = false;
            ResetProgress();
        }
    }

    private void Start()
    {
        spawnItemTimer = 5f;

        // Sync initial state from whatever was set on currentItemRequest in the inspector
        canRequest = currentItemRequest == null;
        SetRequestActive(!canRequest);

        if (requestActive)
        {
            BeginRequest();
        }
    }

    private void Update()
    {
        if (isPlayerHoldingButtonDown)
        {
            HandleHoldLogic();
        }

        canRequest = currentItemRequest == null;

        if (canRequest)
        {
            SetRequestActive(false);
            HandleSpawnCountdown();
        }
        else
        {
            SetRequestActive(true);
            HandleRequestCountdown();
        }
    }

    private void HandleSpawnCountdown()
    {
        spawnItemTimer -= Time.deltaTime;
        if (spawnItemTimer > 0f) return;

        spawnItemTimer = spawnItemTimerMax;

        if (presentTest)
        {
            SpawnNewItem();
        }
        else
        {
            currentItemRequest = validItemRequestsSO.itemRequestSOList[
                UnityEngine.Random.Range(0, validItemRequestsSO.itemRequestSOList.Count)];
        }

        BeginRequest();
    }

    private void BeginRequest()
    {
        if (currentItemRequest == null) return;

        timerFinalized = false;
        float maxTime = UnityEngine.Random.Range(randTimerMin, randTimerMax);
        timerValue = maxTime;

        Debug.Log(currentItemRequest.requestName);

        if (requestUI != null)
        {
            requestUI.ShowRequest(currentItemRequest.requestName, maxTime);
        }
    }

    private void HandleRequestCountdown()
    {
        if (timerFinalized) return;

        if (timerValue > 0f)
        {
            timerValue -= Time.deltaTime;
            if (requestUI != null) requestUI.UpdateTimer(timerValue);
        }
        else
        {
            FailRequest();
        }
    }

    private void SetRequestActive(bool active)
    {
        if (requestActive == active) return;
        requestActive = active;

        if (!active && requestUI != null)
        {
            requestUI.HideAll();
        }
    }

    private void SpawnNewItem()
    {
        // Spawns items in list order
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

        if (buttonPressProgress >= itemSO.targetGoal)
        {
            CompleteDelivery();
        }

        // For progress bar — look at fill station.
        float inputProgress = (float)buttonPressProgress / itemSO.targetGoal;
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
        if (holdProgressTimer >= itemSO.targetGoal)
        {
            CompleteDelivery();
        }
    }

    private void AddPoints()
    {
        string itemName = currentItemRequest.requestName;
        switch (itemName)
        {
            case "Sunscreen":
                manager.AddScore(5);
                break;
            case "CoconutDrinkFilled":
                manager.AddScore(10);
                break;
            case "Towel":
                manager.AddScore(10);
                break;
            case "FullPokeBowl":
                manager.AddScore(20);
                break;
        }
    }

    private void CompleteDelivery()
    {
        if (manager != null)
        {
            AddPoints();
        }

        timerFinalized = true;
        if (requestUI != null) requestUI.ShowSuccess();
        OnRequestSucceeded?.Invoke();

        isPlayerHoldingButtonDown = false;
        currentItemRequest = null;

        PC.GetItemObject().DestroySelf();
        Debug.Log("Item Delivered!");

        ResetProgress();
    }

    private void FailRequest()
    {
        timerFinalized = true;
        if (requestUI != null) requestUI.ShowFail();
        OnRequestFailed?.Invoke();

        // Cleared so a new request can start after this one times out.
        currentItemRequest = null;

        ResetProgress();
    }

    private void HandleWrongItemPenalty()
    {
        if (!wrongItemPenaltyApplied)
        {
            Debug.Log("Wrong Item!");
            wrongItemPenaltyApplied = true;
            // Timer penalty
            // Sound triggers
            // Visual red outline on timer & shake?
        }
    }

    private void ResetProgress()
    {
        spawnItemTimerMax = UnityEngine.Random.Range(4, 7);
        buttonPressProgress = 0;
        spawnItemTimer = spawnItemTimerMax;
        holdProgressTimer = 0f;
    }
}