using UnityEngine;
using UnityEngine.UI;

public class TEMPTouristsTimer : MonoBehaviour
{
    private Slider slider;

    public float countdownDuration = 10f; // Time in seconds to go from full to empty
    public bool loop = false;

    private void Start()
    {
        slider = GetComponent<Slider>();
        
        slider.minValue = 0f;
        slider.maxValue = countdownDuration;
        slider.value = countdownDuration; 
    }

    private void Update()
    {
        if (slider.value > 0f)
        {
            slider.value -= Time.deltaTime;
        }
        else if (loop)
        {
            slider.value = countdownDuration;
        }
        else
        {
            Debug.Log("Slider reached zero!");
        }
    }
}
