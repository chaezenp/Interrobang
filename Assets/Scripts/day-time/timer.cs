using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    [Header("Timer Settings")]
    public float startingTime = 300f; // 5 minutes

    [Header("UI")]
    public TextMeshProUGUI timerText;

    private float currentTime;
    private bool timerRunning = true;

    private void Start()
    {
        currentTime = startingTime;
    }

    private void Update()
    {
        if (!timerRunning)
            return;

        currentTime -= Time.deltaTime;

        if (currentTime <= 0)
        {
            currentTime = 0;
            timerRunning = false;

            TimeUp();
        }

        UpdateTimerDisplay();
    }

    private void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void TimeUp()
    {
        Debug.Log("Time Up!");

        // Put lose/win screen code here
    }
}