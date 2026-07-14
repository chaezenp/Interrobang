using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    [Header("Info")]
    public string itemName;

    public Sprite icon;

    [Header("Held Prefab")]
    public GameObject heldPrefab;

    [Header("Task Info")]
    public bool canCompleteTask;
}