using UnityEngine;

public class AnalogClock : MonoBehaviour
{
    public RectTransform hourHand;
    public RectTransform minuteHand;
    public RectTransform secondHand;

    public float dayLengthInSeconds = 300f; // 5-minute day

    private float elapsedTime;

    void Update()
    {
        elapsedTime += Time.deltaTime;

        float dayProgress = (elapsedTime % dayLengthInSeconds) / dayLengthInSeconds;

        // Full day = 12 hours
        float totalHours = dayProgress * 12f;

        float hours = totalHours;
        float minutes = (totalHours % 1f) * 60f;
        float seconds = (minutes % 1f) * 60f;

        hourHand.localRotation = Quaternion.Euler(0, 0, -hours * 30f);
        minuteHand.localRotation = Quaternion.Euler(0, 0, -minutes * 6f);
        secondHand.localRotation = Quaternion.Euler(0, 0, -seconds * 6f);
    }
}