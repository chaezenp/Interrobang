using System.Collections.Generic;
using UnityEngine;

public class BowlItemObject : ItemObject 
{
    // Define the exact required order here (e.g., Index 0 = Rice SO, Index 1 = Fish SO)
    [SerializeField] private List<ItemSO> recipeOrderSOList; 
    [SerializeField] private List<ItemSO> bowlProgressionSOList; 

    private List<ItemSO> itemObjectSOList;

    private void Awake() 
    {
        if (itemObjectSOList == null)
        {
            itemObjectSOList = new List<ItemSO>();
        }
    }

    public bool TryAddIngredient(ItemSO itemSO) 
    {
        // Check if the recipe is already full
        int currentCount = itemObjectSOList.Count;
        if (currentCount >= recipeOrderSOList.Count) return false;

        // The incoming item must match the exact index position expected next
        if (recipeOrderSOList[currentCount] != itemSO) 
        {
            Debug.LogWarning($"Wrong ingredient order! Expected: {recipeOrderSOList[currentCount].name}, Got: {itemSO.name}");
            return false;
        }
        
        // Prevent adding duplicate ingredients
        if (itemObjectSOList.Contains(itemSO)) return false;

        // Log the new ingredient into our active array
        itemObjectSOList.Add(itemSO);

        // Calculate which stage prefab we need to spawn next
        int currentStageIndex = itemObjectSOList.Count - 1;

        if (currentStageIndex < bowlProgressionSOList.Count)
        {
            IItemObjectParent currentParent = GetItemObjectParent();
            List<ItemSO> ingredientsToTransfer = new List<ItemSO>(itemObjectSOList);

            DestroySelf();

            ItemObject newBowlInstance = ItemObject.SpawnItemObject(bowlProgressionSOList[currentStageIndex], currentParent);

            if (newBowlInstance.TryGetComponent<BowlItemObject>(out BowlItemObject newBowlItemObject))
            {
                newBowlItemObject.SetIngredientList(ingredientsToTransfer);
            }

            return true;
        }

        return false;
    }

    public void SetIngredientList(List<ItemSO> savedList)
    {
        this.itemObjectSOList = savedList;
    }
}
