using UnityEngine;

[CreateAssetMenu()]
public class ItemSO : ScriptableObject
{
    public Transform prefab;
    public Sprite icon;

    public string objectName;

    public bool isHoldItem;

    public int targetGoal;

    public float timeLimit; 

}

