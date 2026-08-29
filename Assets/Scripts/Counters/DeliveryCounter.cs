using System;
using UnityEngine;

public class DeliveryCounter : BaseCounter, IHasProgress
{
    public event EventHandler <IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    public static event EventHandler OnCorrectItemDelivery;
    public static event EventHandler OnWrongItemDelivery;

    public static DeliveryCounter Instance {get; private set; }


    [Header("Requests")]
    [SerializeField] private ValidItemRequestsSO validItemRequestsSO;
    [SerializeField] private ItemRequestSO currentItemRequest;
    [SerializeField] private TouristRequestUI requestUI;
    [SerializeField] private RequestInputControlsUI requestInputUI;
    [SerializeField] private TouristAngerChase angerChase;
    [SerializeField] private WrongPenaltyUI wrongPenaltyUI;


    public PlayerController PC;

    // For presentations: cycle through the request list in order instead of randomly
    [Header("Present test bools")]
    public bool presentTest;
    public bool TouristLeaveTEST;
    public int currentItemIndex = 0;

    [Header("Timer")]
    public int randTimerMin = 30;
    public int randTimerMax = 60;
    public float wrongItemTimDeduction = 5f;

    [Header("Leaving")]
    public int minDeliveriesBeforeLeave = 3;
    public int maxDeliveriesBeforeLeave = 6;

    public event Action OnRequestFailed;
    public event Action OnRequestSucceeded;

    // Fired once this tourist has hit its randomized delivery quota and is ready to leave.
    public event Action OnReadyToLeave;

    private TouristManager manager;
    private DifficultyManager difficultyManager;

    private bool canRequest = false;
    private bool requestActive = false;
    private bool timerFinalized = false;
    private bool waitingForChaseToEnd = false;
    private bool isReadyToLeave = false;
    private float timerValue;

    private int successfulDeliveries = 0;
    private int deliveriesUntilLeave;

    private float spawnItemTimer;
    private float spawnItemTimerMax = 4f;
    private float leaveTimerMax = 5f;

    private int buttonPressProgress;
    private float holdProgressTimer = 0f;
    private bool isPlayerHoldingButtonDown = false;
    private bool wrongItemPenaltyApplied = false;

    //Points
    [Header("Points for items; ONLY EDIT IN TOURIST MANAGER")]
    public int SunscreenPoints;
    public int CoconutPoints;
    public int TowelPoints;
    public int PokePoints;

    private void OnEnable()
    {
        if (angerChase != null)
        {
            angerChase.OnChaseEnded += HandleChaseEnded;
        }
    }

    private void OnDisable()
    {
        if (angerChase != null)
        {
            angerChase.OnChaseEnded -= HandleChaseEnded;
        }
    }

    private void HandleChaseEnded()
    {
        waitingForChaseToEnd = false;
        spawnItemTimer = spawnItemTimerMax;
    }

    public override void Interact(PlayerController playerController)
    {
        if (!playerController.HasItemObject())
        {
            // Player not holding anything
            return;
        }

        ItemSO itemSO = playerController.GetItemObject().GetItemObjectSO();
        Debug.Log(itemSO);
        wrongItemPenaltyApplied = false;

        if (currentItemRequest != null && !currentItemRequest.itemSOList.Contains(itemSO))
        {
            HandleWrongItemPenalty();
            return;
        }

        if (itemSO.isHoldItem)
        {
           //isPlayerHoldingButtonDown = true;
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
    public void InitializeDiffManager(DifficultyManager managerReference)
    {
        difficultyManager = managerReference;
    }

    public override void InteractHold(PlayerController playerController)
    {
        ItemSO itemSO = playerController.GetItemObject().GetItemObjectSO();

        if (currentItemRequest != null && !currentItemRequest.itemSOList.Contains(itemSO))
        {
            HandleWrongItemPenalty();
            return;
        }
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

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        spawnItemTimer = 5f;
        if (!presentTest){
        deliveriesUntilLeave = UnityEngine.Random.Range(minDeliveriesBeforeLeave, maxDeliveriesBeforeLeave);
        }
        else
        {
            deliveriesUntilLeave = 4;
        }

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
        if (isReadyToLeave || TouristLeaveTEST)
        {
            touristLeave();
        }
    }

    private void HandleSpawnCountdown()
    {
        // No item request until not chasing, and none once the delivery quota is hit.
        if (waitingForChaseToEnd || isReadyToLeave) return;

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
        TellManagerWhatItem();

        if (requestUI != null)
        {
            requestUI.ShowRequest(currentItemRequest.requestName, maxTime);
        }
        if (requestInputUI != null)
        {
            requestInputUI.TellUIWhatItem(currentItemRequest.requestName);
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

        // For progress bar — look at fill station
        float inputProgress = (float)buttonPressProgress / itemSO.targetGoal;
        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            progressNormalized = inputProgress
        });

        if (requestInputUI != null)
        {
            requestInputUI.ChangeTapNumber(buttonPressProgress);
        }

        if (buttonPressProgress >= itemSO.targetGoal)
        {
            CompleteDelivery();
        }
    }

    private void HandleHoldLogic()
    {
        if (!PC.HasItemObject())
        {
            InteractHoldRelease(PC);
            return;
        }

        ItemSO itemSO = PC.GetItemObject().GetItemObjectSO();

        if (!itemSO.isHoldItem)
        {
            InteractHoldRelease(PC);
            return;
        }

        holdProgressTimer += Time.deltaTime;
        float testforHold = holdProgressTimer/itemSO.targetGoal;
        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            progressNormalized = holdProgressTimer / itemSO.targetGoal
        });
            Debug.Log("Pro: " + testforHold);


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
                manager.AddScore(SunscreenPoints);
                break;
            case "CoconutDrinkFilled":
                manager.AddScore(CoconutPoints);
                break;
            case "Towel":
                manager.AddScore(TowelPoints);
                break;
            case "FullPokeBowl":
                manager.AddScore(PokePoints);
                break;
        }
    }

    private void TellManagerWhatItem()
    {
        string itemName = currentItemRequest.requestName;
        switch (itemName)
        {
            case "Sunscreen":
                difficultyManager.testFunc();
                break;
            case "CoconutDrinkFilled":
                //difficultyManager.
                break;
            case "Towel":
                //difficultyManager.
                break;
            case "FullPokeBowl":
                //difficultyManager.
                break;
        }
    }

    private void CompleteDelivery()
    {
        if (manager != null)
        {
            AddPoints();
            manager.AddSuccessDeliver(1);
        }

        timerFinalized = true;
        if (requestUI != null) requestUI.ShowSuccess();
        OnRequestSucceeded?.Invoke();
        OnCorrectItemDelivery?.Invoke(this, EventArgs.Empty);

        successfulDeliveries++;
        if (!isReadyToLeave && successfulDeliveries >= deliveriesUntilLeave)
        {
            leaveTimerMax = UnityEngine.Random.Range(4, 15);
            isReadyToLeave = true;
        }

        isPlayerHoldingButtonDown = false;
        currentItemRequest = null;



        PC.GetItemObject().DestroySelf();
        Debug.Log("Item Delivered!");
        ResetProgress();
    }

    private void touristLeave()
    {
        leaveTimerMax -= Time.deltaTime;
        if (leaveTimerMax < 0)
        {
            requestUI.HideAll();
            requestUI.HideWinIcon();
            OnReadyToLeave?.Invoke();
        }
    }

    private void FailRequest()
    {
        timerFinalized = true;
        if (requestUI != null) requestUI.ShowFail();

        // Don't start spawning new requests again until chase is done
        waitingForChaseToEnd = angerChase != null;

        OnRequestFailed?.Invoke();

        // Cleared so a new request can start after this one times out
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
            //timerValue -= wrongItemTimDeduction;
            wrongPenaltyUI.IncorrectItem();
            OnWrongItemDelivery?.Invoke(this, EventArgs.Empty);
        }
    }

    private void ResetProgress()
    {
        spawnItemTimerMax = UnityEngine.Random.Range(4, 7);
        buttonPressProgress = 0;
        spawnItemTimer = spawnItemTimerMax;
        holdProgressTimer = 0f;
        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            progressNormalized = 0f
        });
        if (requestInputUI != null)
        {
            requestInputUI.ResetBools();
            requestInputUI.Hide();
        }
    }
}