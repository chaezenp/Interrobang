using System.Collections.Generic;
using UnityEngine;

public class TouristManager : MonoBehaviour
{
    public static TouristManager Instance { get; private set;}

    private enum SpawnMilestone { Level1, Level2, Level3, Level4, Level5, Level6, Level7, Level8}
    private SpawnMilestone currentMilestone = SpawnMilestone.Level1;

    public GameObject TouristPrefab;
    [SerializeField] private GameObject[] TouristsTypes;
    public GameObject Player;
    public GameObject PlayerModel;
    public GameObject PlayerHandHoldPoint;
    public GameObject ExplodeDeath;

    [SerializeField] private Transform spawnStartPosition;
    [SerializeField] private Transform[] seatPositions;
    [SerializeField] private Transform leaveEndPosition;
    [SerializeField] private Transform playerTransform;

    public int failedRequestsCount = 0;
    public int failsUntilChase = 3;

    private float timerAddPoints = 0;
    [Header("Points System")]

    [Tooltip("Time in seconds until points are added")]
    public float timeNeedSurvivePoints = 20f;
    [Tooltip("Points added based off survived Time")]
    [SerializeField] private int survivedPoints = 10;

    [Header("Item Point Values")]

    [SerializeField] private int SunscreenPoints = 5;
    [SerializeField] private int TowelPoints = 10;
    [SerializeField] private int CoconutPoints = 15;
    [SerializeField] private int PokePoints = 25;

    [Header("Milestone Point Values")]
    [SerializeField] private int PointMilestoneLevel1 = 50;
    [SerializeField] private int PointMilestoneLevel2 = 100;
    [SerializeField] private int PointMilestoneLevel3 = 250;
    [SerializeField] private int PointMilestoneLevel4 = 500;
    [SerializeField] private int PointMilestoneLevel5 = 1000;
    [SerializeField] private int PointMilestoneLevel6 = 1500;
    [SerializeField] private int PointMilestoneLevel7 = 2000;



    [Header("Leaving")]
    public int minDeliveriesBeforeLeave = 3;
    public int maxDeliveriesBeforeLeave = 6;
    [Tooltip("Dont go lower than 15")]
    public float spawnCooldownAfterLeave = 15f;

    [Header("Active Tourist Target")]
    // How many tourists should be active at once at each milestone.
    public int[] targetActiveTourists = new int[] { 1, 2, 3, 4, 5, 6, 7, 8 };

    private List<Transform> availableSlots = new List<Transform>();
    private List<DeliveryCounter> ActiveTourists = new List<DeliveryCounter>();
    private bool gameModeIsChase = false;
    private int totalPoints = 0;
    private float spawnCooldownTimer = 0f;
    private int successfulDeliveriesAmount = 0;
    private bool playerCaught = false;

    private void Start()
    {
        if (spawnCooldownAfterLeave < 15)
        {
            spawnCooldownTimer = 15f;
        }
        availableSlots.AddRange(seatPositions);
        CheckPointsAndSlots();
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        timerAddPoints += Time.deltaTime;
        if (timerAddPoints > timeNeedSurvivePoints)
        {
            AddScore(survivedPoints);
        }

        if (spawnCooldownTimer > 0f && (ActiveTourists.Count < GetTargetActiveTourists()))
        {
            spawnCooldownTimer -= Time.deltaTime;
            if (spawnCooldownTimer <= 0f)
            {
                spawnCooldownTimer = 0f;
                CheckPointsAndSlots();
            }
        }
    }

    public void AddScore(int amount)
    {
        timerAddPoints = 0;
        totalPoints += amount;
        //Debug.Log($"Score updated: {totalPoints}. Checking milestones...");
        
        CheckPointsAndSlots();
    }

    public void AddSuccessDeliver(int amount)
    {
        successfulDeliveriesAmount += amount;
    }

    private void CheckPointsAndSlots()
    {
        if (playerCaught) return;
        if (gameModeIsChase) return;
        UpdateMilestone();
        if (spawnCooldownTimer > 0f) return;

        if (availableSlots.Count == 0)
        {
            //Debug.Log("Want to spawn but seats are full, waiting for an empty seat");
            return;
        }

        int target = GetTargetActiveTourists();
        Debug.Log("active target: " + target);

        if (ActiveTourists.Count < target)
        {
            SpawnTourist();
            // Space out spawns
            spawnCooldownTimer = spawnCooldownAfterLeave;
        }
    }

    private void UpdateMilestone()
    {
        while (true)
        {
            switch (currentMilestone)
            {
                case SpawnMilestone.Level1 when totalPoints >= PointMilestoneLevel1:
                    spawnCooldownAfterLeave = 15f;
                    currentMilestone = SpawnMilestone.Level2;
                    continue;

                case SpawnMilestone.Level2 when totalPoints >= PointMilestoneLevel2:
                    currentMilestone = SpawnMilestone.Level3;
                    continue;

                case SpawnMilestone.Level3 when totalPoints >= PointMilestoneLevel3:
                    spawnCooldownAfterLeave = 10f;
                    currentMilestone = SpawnMilestone.Level4;
                    continue;

                case SpawnMilestone.Level4 when totalPoints >= PointMilestoneLevel4:
                    currentMilestone = SpawnMilestone.Level5;
                    continue;
                case SpawnMilestone.Level5 when totalPoints >= PointMilestoneLevel5:
                    currentMilestone = SpawnMilestone.Level6;
                    continue;            
                case SpawnMilestone.Level6 when totalPoints >= PointMilestoneLevel6:
                    spawnCooldownAfterLeave = 5f;
                    currentMilestone = SpawnMilestone.Level7;
                    continue;
                case SpawnMilestone.Level7 when totalPoints >= PointMilestoneLevel7:
                    currentMilestone = SpawnMilestone.Level8;
                    continue;
                    }
            break;
        }

    }

    private int GetTargetActiveTourists()
    {
        if (targetActiveTourists == null || targetActiveTourists.Length == 0) return 1;
        int index = Mathf.Clamp((int)currentMilestone, 0, targetActiveTourists.Length - 1);
        Debug.Log("active index: " + index);
        return targetActiveTourists[index];
    }

    private void SpawnTourist()
    {
        int slotIndex = Random.Range(0, availableSlots.Count);
        Transform targetSlot = availableSlots[slotIndex];
        availableSlots.RemoveAt(slotIndex);

        GameObject newNPC = Instantiate(TouristPrefab, spawnStartPosition.position, Quaternion.identity);

        Transform modelsParent = newNPC.transform.Find("Models");

        if (modelsParent != null)
        {
            SelectRandomModel(modelsParent);
        }
        else
        {
            Debug.LogError("Could not find a child object named 'Models' on the spawned NPC!");
        }

        
        DeliveryCounter touristScript = newNPC.GetComponent<DeliveryCounter>();
        PlayerController PC = Player.GetComponent<PlayerController>();
        touristScript.PC = PC;
        touristScript.Initialize(this);
        touristScript.minDeliveriesBeforeLeave = minDeliveriesBeforeLeave;
        touristScript.maxDeliveriesBeforeLeave = maxDeliveriesBeforeLeave;
        touristScript.SunscreenPoints = SunscreenPoints;
        touristScript.TowelPoints = TowelPoints;
        touristScript.CoconutPoints = CoconutPoints;
        touristScript.PokePoints = PokePoints;

        touristScript.OnReadyToLeave += () => OnNPCLeft(newNPC);
        ActiveTourists.Add(touristScript);

        TouristAngerChase touristAngerScript = newNPC.GetComponent<TouristAngerChase>();
        if (touristAngerScript != null)
        {
            touristAngerScript.Player = Player;
            touristAngerScript.PlayerModel = PlayerModel;
            touristAngerScript.playerHand = PlayerHandHoldPoint;
            touristAngerScript.seatPos = targetSlot;
            touristAngerScript.LeavePos = leaveEndPosition;
        }

        if (ExplodeDeath != null && touristAngerScript != null)
        {
            touristAngerScript.explodeDeath = ExplodeDeath;
        }
        
    }

    void SelectRandomModel(Transform modelsParent)
    {
        int totalModels = modelsParent.childCount;
        if (totalModels == 0) return;

        // Pick a random child index
        int randomIndex = Random.Range(0, totalModels);

        // Loop through and activate only the chosen one
        for (int i = 0; i < totalModels; i++)
        {
            modelsParent.GetChild(i).gameObject.SetActive(i == randomIndex);
        }
    }

    public void touristRequestFailed(GameObject touristGo)
    {
        failedRequestsCount++;
        if (failedRequestsCount >= failsUntilChase && !gameModeIsChase)
        {
            TriggeredChaseMode();
        }
    }

    private void TriggeredChaseMode()
    {
        gameModeIsChase = true;
    }

    public void OnNPCLeft(GameObject npcGo)
    {
        DeliveryCounter script = npcGo.GetComponent<DeliveryCounter>();
        if (ActiveTourists.Contains(script)) 
            ActiveTourists.Remove(script);

        TouristAngerChase angerScript = npcGo.GetComponent<TouristAngerChase>();
        if (angerScript != null && angerScript.seatPos != null)
        {
            if (!availableSlots.Contains(angerScript.seatPos))
            {
                availableSlots.Add(angerScript.seatPos);

                angerScript.LeaveHotel();
            }
        }

        spawnCooldownTimer = spawnCooldownAfterLeave;

        CheckPointsAndSlots();
    }

    public int GetSuccessfulDeliveriesAmount()
    {
        return successfulDeliveriesAmount;
    }
    
    public bool isPlayerCaught()
    {
        return playerCaught == true;
    }
}
