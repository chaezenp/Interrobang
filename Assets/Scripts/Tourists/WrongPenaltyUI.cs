using UnityEngine; 
using UnityEngine.UI; 

public class WrongPenaltyUI : MonoBehaviour 
{ 
    [Header("UI References")] 
    [SerializeField] private Slider targetSlider; 
    [SerializeField] private RawImage fillOrBackgroundToFlash; 

    [Header("Shake Settings")] 
    [SerializeField] private float shakeDuration = 0.5f; 
    [SerializeField] private float shakeMagnitude = 1f; 
    [Tooltip("Higher numbers mean faster oscillation, lower numbers make it a gentle sway.")]
    [SerializeField] private float shakeSpeed = 10f;
    [Header("Flash Settings")] 
    [SerializeField] private Color flashColor = Color.red; 
    [SerializeField] private float flashDuration = 0.5f; 

    private Vector3 originalPosition; 
    private Color originalColor; 

    private float shakeTimer; 
    private float flashTimer; 

    private void Start() 
    { 
        if (targetSlider != null) 
        { 
            originalPosition = targetSlider.transform.localPosition; 
        } 
        if (fillOrBackgroundToFlash != null) 
        { 
            originalColor = fillOrBackgroundToFlash.color; 
        } 
    } 

    public void IncorrectItem() 
    { 
        shakeTimer = shakeDuration; 
        flashTimer = flashDuration; 
        if (fillOrBackgroundToFlash != null) 
        { 
            fillOrBackgroundToFlash.color = flashColor; 
        } 
    } 

    private void Update() 
    { 
        HandleShake(); 
        HandleFlash(); 
    } 

    private void HandleShake() 
    { 
        if (shakeTimer > 0) 
        { 
            shakeTimer -= Time.deltaTime; 

            if (shakeTimer <= 0) 
            { 
                targetSlider.transform.localPosition = originalPosition; 
            } 
            else 
            { 
                float wave = Mathf.Sin(Time.time * shakeSpeed);
                float xOffset = wave * shakeMagnitude; 

                targetSlider.transform.localPosition = new Vector3(originalPosition.x + xOffset, originalPosition.y, originalPosition.z); 
            } 
        } 
    } 

    private void HandleFlash() 
    { 
        if (flashTimer > 0) 
        { 
            flashTimer -= Time.deltaTime; 
            if (flashTimer <= 0) 
            { 
                fillOrBackgroundToFlash.color = originalColor; 
            } 
            else 
            { 
                float t = 1f - (flashTimer / flashDuration); 
                fillOrBackgroundToFlash.color = Color.Lerp(flashColor, originalColor, t); 
            } 
        } 
    } 
}
