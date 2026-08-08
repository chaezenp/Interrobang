using System;
using UnityEngine;
using UnityEngine.UI;

public class TouristRequestUI : MonoBehaviour
{
    [SerializeField] private Texture2D[] itemTextures;
    [SerializeField] private Slider slider;
    [SerializeField] private GameObject sliderInstance;
    [SerializeField] private RawImage requestItemImage;

    [SerializeField] private GameObject requestImage;
    [SerializeField] private GameObject check;
    [SerializeField] private GameObject failX;
 
    public bool IsFailIconActive => failX != null && failX.activeInHierarchy;
 
    private void Awake()
    {
        if (check != null) check.SetActive(false);
        if (failX != null) failX.SetActive(false);
        if (sliderInstance != null) sliderInstance.SetActive(false);
        if (requestImage != null) requestImage.SetActive(false);
    }
 
    // When new request happens show UI stuff
    public void ShowRequest(string itemName, float maxTime)
    {
        if (check != null) check.SetActive(false);
        if (failX != null) failX.SetActive(false);
 
        if (slider != null)
        {
            slider.minValue = 0f;
            slider.maxValue = maxTime;
            slider.value = maxTime;
        }
 
        if (sliderInstance != null) sliderInstance.SetActive(true);
        if (requestImage != null) requestImage.SetActive(true);
 
        SwitchRawImage(itemName);
    }
 
    public void UpdateTimer(float value)
    {
        if (slider != null) slider.value = value;
    }
 
    public void ShowSuccess()
    {
        if (requestImage != null) requestImage.SetActive(false);
        if (sliderInstance != null) sliderInstance.SetActive(false);
        if (check != null) check.SetActive(true);
    }
 
    public void ShowFail()
    {
        if (requestImage != null) requestImage.SetActive(false);
        if (sliderInstance != null) sliderInstance.SetActive(false);
        if (failX != null) failX.SetActive(true);
    }
 
    // No active request at all
    public void HideAll()
    {
        //slider.value = 0f;
        if (requestImage != null) requestImage.SetActive(false);
        if (sliderInstance != null) sliderInstance.SetActive(false);
    }
 
    // Turns off failX when tourist start chasing
    public void HideFailIcon()
    {
        if (failX != null) failX.SetActive(false);
    }
 
    public void SwitchRawImage(string textureName)
    {
        if (itemTextures == null || itemTextures.Length == 0 || requestItemImage == null || textureName == null)
            return;
 
        string cleanSearchName = textureName.Trim();
 
        foreach (Texture2D tex in itemTextures)
        {
            if (tex != null && tex.name.Trim().Equals(cleanSearchName, StringComparison.OrdinalIgnoreCase))
            {
                requestItemImage.texture = tex;
                return;
            }
        }
 
        requestItemImage.texture = itemTextures[0];
    }
}