using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class TouristsTimer : MonoBehaviour
{
    [SerializeField] private Slider slider;

    public float countdownDuration = 10f; // Time in seconds to go from full to empty
    public bool _recievedItemOnTime = false;
     public GameObject sliderInstance; 
     public GameObject check;
     public GameObject failX;
     public GameObject _item;
     public GameObject Player;
     public GameObject ExplodeDeath;
     

    private void Start()
    {        
        slider.minValue = 0f;
        slider.maxValue = countdownDuration;
        slider.value = countdownDuration; 
    }

    private void Update()
    {
        if (slider.value > 0f)
        {
            slider.value -= Time.deltaTime;
            if (!_item)
            {
            _recievedItemOnTime = true;
            OnWin(_recievedItemOnTime);
            }
        }

        else
        {
            Debug.Log("Slider reached zero!");
            if (_item != null)
            {
            _recievedItemOnTime = false;
            OnFail(_recievedItemOnTime);
            Debug.Log("FAIL!");
            }

        }

    }

    private void OnFail(bool _recievedItemOnTime)
    {
        if (_recievedItemOnTime == false)
        {
            failX.SetActive(true);
            sliderInstance.SetActive(false);    
            if(Player != null && ExplodeDeath != null)
            {
            Player.SetActive(false);
            ExplodeDeath.SetActive(true);
            }
        }
    }

        private void OnWin(bool _recievedItemOnTime)
    {
        if (_recievedItemOnTime == true)
        {
            check.SetActive(true);
            sliderInstance.SetActive(false);    
        }
    }
}
