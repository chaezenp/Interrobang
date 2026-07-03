using UnityEngine;
using UnityEngine.UIElements;

public class UIButtonTrigger : MonoBehaviour
{
    public GameObject _UIbuttonTrigger;
    public GameObject _Check;
    public GameObject _failX;
    public GameObject theSecondImageEnabled;
    public GameObject currentPickableItem;
    public bool isEnabled = false;

    private void Update()
    {
        if (_Check.activeSelf || _failX.activeSelf)
        {
            isEnabled = false;
        }
        
        _UIbuttonTrigger.SetActive(isEnabled);
    if (currentPickableItem == null && theSecondImageEnabled != null && theSecondImageEnabled.activeSelf)
    {
        theSecondImageEnabled.SetActive(false);
    }   
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isEnabled = true;
        }

        if (other.CompareTag("Pickable") && theSecondImageEnabled != null)
        {
            theSecondImageEnabled.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // pick up item
        if (other.CompareTag("Player") && isEnabled)
        {
            isEnabled = false;
        }

        if (other.CompareTag("Pickable") && theSecondImageEnabled != null)
        {
            theSecondImageEnabled.SetActive(false);
        }
    }



}
