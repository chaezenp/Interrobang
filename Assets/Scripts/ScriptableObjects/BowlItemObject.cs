using System.Collections.Generic;
using UnityEngine;

public class BowlItemObject : ItemObject 
{
    [SerializeField] private List<ItemSO> validItemObjectSOList;
    [SerializeField] private List<ItemSO> bowlProgressionSOList; 

    private List<ItemSO> itemObjectSOList;

    private void Awake() 
    {
        // Only initialize a new list if one wasn't passed down from a previous state
        if (itemObjectSOList == null)
        {
            itemObjectSOList = new List<ItemSO>();
        }
    }

    public bool TryAddIngredient(ItemSO itemSO) 
    {
        // 1. Verify this specific ingredient is allowed in the final recipe
        if (!validItemObjectSOList.Contains(itemSO)) return false;
        
        // 2. Prevent adding duplicate ingredients (e.g., adding Rice twice)
        if (itemObjectSOList.Contains(itemSO)) return false;

        // 3. Log the new ingredient into our active array
        itemObjectSOList.Add(itemSO);

        // 4. Calculate which stage prefab we need to spawn next
        // 1st ingredient added = index 0 (RiceBowl), 2nd added = index 1 (FullPokeBowl)
        int currentStageIndex = itemObjectSOList.Count - 1;

        if (currentStageIndex < bowlProgressionSOList.Count)
        {
            // Keep a record of the parent attachment and the ingredients array
            IItemObjectParent currentParent = GetItemObjectParent();
            List<ItemSO> ingredientsToTransfer = new List<ItemSO>(itemObjectSOList);

            // Clear the old version from the player's hands or counter top
            DestroySelf();

            // Bring the upgraded visual prefab version into the game world
            ItemObject newBowlInstance = ItemObject.SpawnItemObject(bowlProgressionSOList[currentStageIndex], currentParent);

            // Pass the ingredients list forward so the new prefab knows its history
            if (newBowlInstance.TryGetComponent<BowlItemObject>(out BowlItemObject newBowlItemObject))
            {
                newBowlItemObject.SetIngredientList(ingredientsToTransfer);
            }

            return true;
        }

        return false;
    }

    // Helper function to inject data from the destroyed bowl version
    public void SetIngredientList(List<ItemSO> savedList)
    {
        this.itemObjectSOList = savedList;
    }
}
