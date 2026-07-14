using System;
using UnityEngine;
using UnityEngine.UI;

public class TouristsTimer : MonoBehaviour
{
    [SerializeField] private Texture2D[] itemTextures;
    [SerializeField] private Slider slider;
    public string itemName;

    public float countdownDuration = 10f; // Time in seconds to go from full to empty
    public bool _recievedItemOnTime = false;
    public GameObject sliderInstance; 
    public GameObject requestImage;
    public GameObject check;
    public GameObject failX;
    public GameObject Player;
    
    
    //public GameObject ExplodeDeath;

    public bool spawnedinYET = false;
    public RawImage requestItemImage;
    private bool _requestActive = false;
    public bool requestActive
    {
        get => _requestActive;
        set
        {
            // Only trigger changes if the state actually changes
            if (_requestActive != value)
            {
                _requestActive = value;
                if (_requestActive)
                    OnRequestStarted();
                else
                    RequestImageDisabled();
            }
        }
    }

    private bool timerFinalized = false;

    private void Start()
    {
        slider.minValue = 0f;
        slider.maxValue = countdownDuration;
        slider.value = countdownDuration;
        
        check.SetActive(false);
        failX.SetActive(false);
    }

    private void Update()
    {
        if (!requestActive || timerFinalized) return;

        if (slider.value > 0f)
        {
            slider.value -= Time.deltaTime;
        }
        else
        {
            _recievedItemOnTime = false;
            OnFail();
        }
    }

    public void DeliverItem(bool isCorrectItem)
    {
        if (!requestActive || timerFinalized) return;

        if (isCorrectItem)
        {
            _recievedItemOnTime = true;
            OnWin();
        }
        else
        {
            _recievedItemOnTime = false;
            OnFail();
        }
    }

    private void OnRequestStarted()
    {
        timerFinalized = false;
        slider.value = countdownDuration;
        
        check.SetActive(false);
        failX.SetActive(false);
        sliderInstance.SetActive(true);
        if (requestImage != null) requestImage.SetActive(true);

        SwitchRawImage(itemName);
    }

    private void OnFail()
    {
        timerFinalized = true;
        if (requestImage != null) requestImage.SetActive(false);
        
        failX.SetActive(true);
        sliderInstance.SetActive(false);
    }

    private void OnWin()
    {
        timerFinalized = true;
        if (requestImage != null) requestImage.SetActive(false);
        
        check.SetActive(true);
        sliderInstance.SetActive(false);
    }

    private void RequestImageDisabled()
    {
        if (requestImage != null) requestImage.SetActive(false);
        sliderInstance.SetActive(false);
    }
    public void SwitchRawImage(string textureName)
    {
        if (itemTextures == null || itemTextures.Length == 0 || requestItemImage == null) return;

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
