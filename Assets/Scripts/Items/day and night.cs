using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Header("References")]
    public Transform sunPivot;
    public Light sun;

    [Header("Time")]
    public float dayLength = 120f;

    [Range(0, 24)]
    public float startHour = 9f;

    [Header("Lighting")]
    public float maxIntensity = 1.5f;
    public float minIntensity = 0f;

    private float currentTime;
    private float rotationSpeed;

    void Start()
    {
        if (sunPivot == null || sun == null)
        {
            Debug.LogError("Assign the Sun Pivot and Directional Light!");
            enabled = false;
            return;
        }

        rotationSpeed = 360f / dayLength;

        // Convert start hour to elapsed time
        currentTime = (startHour / 24f) * dayLength;

        // Set the starting rotation
        float angle = (startHour / 24f) * 360f - 90f;
        sunPivot.localRotation = Quaternion.Euler(angle, 0f, 0f);
    }

    void Update()
    {
        currentTime += Time.deltaTime;

        if (currentTime >= dayLength)
            currentTime = 0f;

        float timePercent = currentTime / dayLength;

        // Rotate ONLY the pivot
        float angle = (timePercent * 360f) - 90f;
        sunPivot.localRotation = Quaternion.Euler(angle, 0f, 0f);

        // Brightness
        float intensity = Mathf.Clamp01(Mathf.Sin(timePercent * Mathf.PI));

        sun.intensity = Mathf.Lerp(minIntensity, maxIntensity, intensity);
    }
}