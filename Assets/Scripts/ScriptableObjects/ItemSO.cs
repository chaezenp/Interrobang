using UnityEngine;

[CreateAssetMenu()]
public class ItemSO : ScriptableObject
{
    public Transform prefab;
    public Sprite icon;

    public string objectName;

    public bool isHoldItem;

    [Header("If item has hold input (hold bool),")]
    [Header("then target goal will be how many seconds to hold")]
    [Header("If hold bool is unchecked,")]
    [Header("then target goal will be how many presses to deliver")]

    public int targetGoal;
}

