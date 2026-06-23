using System;
using UnityEngine;
using UnityEngine.UI;

public class DestroyWithObject : MonoBehaviour
{
    [Header("Assign the UI Element")]
    public GameObject sliderInstance; 
    [SerializeField] private GameObject check;


    private void OnDestroy()
    {
        if (sliderInstance != null)
        {
            Destroy(sliderInstance);
        }
        if (check != null){
        check.SetActive(true);
        }
    }
}
