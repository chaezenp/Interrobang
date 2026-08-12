using System;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;


public class GameOverUI : MonoBehaviour
{
    public GameObject GameOverMenu;
    public FirstSelectedButton firstSelectedButtonGameOver;
    public CanvasGroup GameOverCanvasGroup;
    [SerializeField] private Button RetryButton;
    [SerializeField] private Button MainMenuButton;

    [SerializeField] private TextMeshProUGUI recipiesDeliveredNumberText;

    [Header("Blur Settings")]
    public Volume blurVolume; 
    public float fadeSpeed = 4f;
    
    private float targetWeight = 1f;
    private bool isGameOver = false;

    private void Awake()
    {
        RetryButton.onClick.AddListener(() =>
        {
            // Click
            Loader.Load(Loader.Scene.TestLevelWithART);
        });
        MainMenuButton.onClick.AddListener(() =>
        {
            // Click
            Loader.Load(Loader.Scene.MainMenuScene);
        });
    }

    private void Start()
    {
        HahaluGameManager.Instance.OnStateChanged += HahaluGameManager_OnStateChanged;
        
        if (blurVolume != null) blurVolume.weight = 0f;
        if (GameOverCanvasGroup != null) GameOverCanvasGroup.alpha = 0f;

        Hide();
    }

    private void HahaluGameManager_OnStateChanged(object sender, EventArgs e)
    {
        if (HahaluGameManager.Instance.IsGameOver())
        {
            Show();
            isGameOver = true;
            recipiesDeliveredNumberText.text = TouristManager.Instance.GetSuccessfulDeliveriesAmount().ToString();
        }
        else
        {
            Hide();
        }
    }

    private void Update()
    {
        if (blurVolume != null && blurVolume.weight != targetWeight && isGameOver)
        {
            float currentWeight = Mathf.MoveTowards(blurVolume.weight, targetWeight, fadeSpeed * Time.unscaledDeltaTime);
            
            blurVolume.weight = currentWeight;
            
            if (GameOverCanvasGroup != null)
            {
                GameOverCanvasGroup.alpha = currentWeight;
            }
        }

        if (blurVolume.weight == targetWeight)
        {
            Time.timeScale = 0f;
        }
    }
    private void Show()
    {
        GameOverMenu.SetActive(true);
        firstSelectedButtonGameOver.FocusMenu();
    }

    private void Hide()
    {
        GameOverMenu.SetActive(false);
    }

}
