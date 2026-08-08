using System.Collections.Generic;
using UnityEngine;

public class TouristManager : MonoBehaviour
{
    private enum SpawnMilestone { Level1, Level2, Level3, Level4, Finished }
    private SpawnMilestone currentMilestone = SpawnMilestone.Level1;

    public GameObject TouristPrefab;
    [SerializeField] private GameObject[] TouristsTypes;
    public GameObject Player;
    public GameObject ExplodeDeath;

    [SerializeField] private Transform spawnStartPosition;
    [SerializeField] private Transform[] seatPositions;
    [SerializeField] private Transform leaveEndPosition;
    [SerializeField] private Transform playerTransform;

    public int failedRequestsCount = 0;
    public int failsUntilChase = 3;
    private float timerAddPoints = 0;
    public float timeNeedSurvivePoints = 20f;
    [SerializeField] private int survivedPoints = 10;

    [Header("Leaving")]
    public int minDeliveriesBeforeLeave = 3;
    public int maxDeliveriesBeforeLeave = 6;
    public float spawnCooldownAfterLeave = 5f;

    [Header("Active Tourist Target")]
    // How many tourists should be active at once at each milestone.
    public int[] targetActiveTourists = new int[] { 1, 2, 3, 4, 5 };

    private List<Transform> availableSlots = new List<Transform>();
    private List<DeliveryCounter> ActiveTourists = new List<DeliveryCounter>();
    private bool gameModeIsChase = false;
    private int totalPoints = 0;
    private float spawnCooldownTimer = 0f;

    private void Start()
    {
        availableSlots.AddRange(seatPositions);
        CheckPointsAndSlots();
    }

    private void Update()
    {
        timerAddPoints += Time.deltaTime;
        if (timerAddPoints > timeNeedSurvivePoints)
        {
            AddScore(survivedPoints);
        }

        if (spawnCooldownTimer > 0f)
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

    private void CheckPointsAndSlots()
    {
        if (gameModeIsChase) return;
        if (spawnCooldownTimer > 0f) return;

        UpdateMilestone();

        if (availableSlots.Count == 0)
        {
            //Debug.Log("Want to spawn but seats are full, waiting for an empty seat");
            return;
        }

        int target = GetTargetActiveTourists();
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
                case SpawnMilestone.Level1 when totalPoints >= 49:
                    currentMilestone = SpawnMilestone.Level2;
                    continue;

                case SpawnMilestone.Level2 when totalPoints >= 99:
                    currentMilestone = SpawnMilestone.Level3;
                    continue;

                case SpawnMilestone.Level3 when totalPoints >= 149:
                    currentMilestone = SpawnMilestone.Level4;
                    continue;

                case SpawnMilestone.Level4 when totalPoints >= 199:
                    currentMilestone = SpawnMilestone.Finished;
                    continue;
            }
            break;
        }
    }

    private int GetTargetActiveTourists()
    {
        if (targetActiveTourists == null || targetActiveTourists.Length == 0) return 1;
        int index = Mathf.Clamp((int)currentMilestone, 0, targetActiveTourists.Length - 1);
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
        touristScript.OnReadyToLeave += () => OnNPCLeft(newNPC);
        ActiveTourists.Add(touristScript);

        TouristAngerChase touristAngerScript = newNPC.GetComponent<TouristAngerChase>();
        if (touristAngerScript != null)
        {
            touristAngerScript.Player = Player;
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
}
