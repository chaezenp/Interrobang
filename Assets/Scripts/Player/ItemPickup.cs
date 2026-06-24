using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public string objectiveTag = "Tourist";

    private bool isHoldingItem = false;
    private Vector3 originalScale;

    private void Start()
    {
        originalScale = transform.localScale; 
    }

    private void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        // pick up item
        if (!isHoldingItem && other.CompareTag("Player"))
        {
            isHoldingItem = true;

            // attach to player 
            transform.SetParent(other.transform);

            // so object not inside player
            transform.localPosition = new Vector3(0f, 1f, 1.2f); 
            transform.localRotation = Quaternion.identity;
            transform.localScale = originalScale;
        }

        // deliver to tourist
        if (isHoldingItem && other.CompareTag(objectiveTag))
        {
            Debug.Log("Item delivered automatically!");
            Destroy(gameObject); 
        }
    }
}
