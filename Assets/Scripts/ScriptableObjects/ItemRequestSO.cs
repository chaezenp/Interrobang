using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
//RecipeSO
public class ItemRequestSO : ScriptableObject
{
    public List<ItemSO> itemSOList;
    public string requestName;
}
