using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TouristManager : MonoBehaviour
{
    public GameObject TouristPrefab;
    public GameObject Player;
    public GameObject ExplodeDeath;
    [SerializeField] private Transform spawnStartPosition;
    [SerializeField] private Transform[] seatPositions;
    [SerializeField] private Transform playerTransform;

    public int failedRequestsCount = 0;
    public int failsUntilChase = 3;

    private List<Transform> availableSlots = new List<Transform>();
    private List<DeliveryCounter> ActiveTourists = new List<DeliveryCounter>();
    private bool gameModeIsChase = false;
    private int totalPoints = 0;

    private void Start()
    {
        availableSlots.AddRange(seatPositions);
        InvokeRepeating(nameof(TrySpawnTourist), 2f, 5f);
    }

    void TrySpawnTourist()
    {
        if (gameModeIsChase) return; 
        if (availableSlots.Count == 0) return;
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
        if (touristAngerScript != null){
        touristAngerScript.Player = Player;
        touristAngerScript.originalPos = targetSlot.transform;
        }
        if (ExplodeDeath != null)
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

        Debug.Log("Too many failures! The customers are revolting!");
    }

    public void OnNPCLeft(GameObject npcGo)
    {
        DeliveryCounter script = npcGo.GetComponent<DeliveryCounter>();
        if (ActiveTourists.Contains(script)) ActiveTourists.Remove(script);
    }

    public void AddScore(int amount)
    {
        totalPoints += amount;
    }
}
