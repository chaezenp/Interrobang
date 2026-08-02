using System.Collections.Generic;
using UnityEngine;

public class TouristManager : MonoBehaviour
{
    private enum SpawnMilestone { Level1, Level2, Level3, Level4, Finished }
    private SpawnMilestone currentMilestone = SpawnMilestone.Level1;

    public GameObject TouristPrefab;
    public GameObject Player;
    public GameObject ExplodeDeath;

    [SerializeField] private Transform spawnStartPosition;
    [SerializeField] private Transform[] seatPositions;
    [SerializeField] private Transform playerTransform;

    public int failedRequestsCount = 0;
    public int failsUntilChase = 3;
    public float timerAddPoints = 0;


    private List<Transform> availableSlots = new List<Transform>();
    private List<DeliveryCounter> ActiveTourists = new List<DeliveryCounter>();
    private bool gameModeIsChase = false;
    private int totalPoints = 0;

    private void Start()
    {
        availableSlots.AddRange(seatPositions);
        SpawnTourist();
    }

    private void Update()
    {
        timerAddPoints += Time.deltaTime;
        if (timerAddPoints > 7)
        {
            AddScore(10);
        }
    }

    public void AddScore(int amount)
    {
        timerAddPoints = 0;
        totalPoints += amount;
        Debug.Log($"Score updated: {totalPoints}. Checking milestones...");
        
        CheckPointsAndSlots();
    }

    private void CheckPointsAndSlots()
    {
        if (gameModeIsChase) return;
        if (availableSlots.Count == 0)
        {
            //Debug.Log("Score reached milestone but seats are full, waiting for an empty seat");
            return;
        }

        switch (currentMilestone)
        {
            case SpawnMilestone.Level1 when totalPoints >= 10:
                currentMilestone = SpawnMilestone.Level2;
                SpawnTourist();
                break;

            case SpawnMilestone.Level2 when totalPoints >= 30:
                currentMilestone = SpawnMilestone.Level3;
                SpawnTourist();
                break;

            case SpawnMilestone.Level3 when totalPoints >= 60:
                currentMilestone = SpawnMilestone.Level4;
                SpawnTourist();
                break;

            case SpawnMilestone.Level4 when totalPoints >= 100:
                currentMilestone = SpawnMilestone.Finished;
                SpawnTourist();
                break;
        }
    }

    private void SpawnTourist()
    {
        int slotIndex = Random.Range(0, availableSlots.Count);
        Transform targetSlot = availableSlots[slotIndex];
        availableSlots.RemoveAt(slotIndex);

        GameObject newNPC = Instantiate(TouristPrefab, spawnStartPosition.position, Quaternion.identity);
        
        DeliveryCounter touristScript = newNPC.GetComponent<DeliveryCounter>();
        PlayerController PC = Player.GetComponent<PlayerController>();
        touristScript.PC = PC;
        touristScript.Initialize(this);
        ActiveTourists.Add(touristScript);

        TouristAngerChase touristAngerScript = newNPC.GetComponent<TouristAngerChase>();
        if (touristAngerScript != null)
        {
            touristAngerScript.Player = Player;
            touristAngerScript.originalPos = targetSlot;
        }

        if (ExplodeDeath != null && touristAngerScript != null)
        {
            touristAngerScript.explodeDeath = ExplodeDeath;
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
        if (angerScript != null && angerScript.originalPos != null)
        {
            if (!availableSlots.Contains(angerScript.originalPos))
            {
                availableSlots.Add(angerScript.originalPos);
                //Debug.Log("A seat has been freed up!");
            }
        }


        CheckPointsAndSlots();
    }
}
