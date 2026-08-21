using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PauseMenuUI : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private FirstSelectedButton firstSelectedButtonPause;
    [SerializeField] private FirstSelectedButton OptionsFirstSelectedButton;
    [SerializeField] private FirstSelectedButton ControlsFirstSelectedButton;
    [SerializeField] private Button ResumeButton;
    [SerializeField] private Button OptionsButton;
    [SerializeField] private Button ControlsButton;
    [SerializeField] private Button QuitButton;

    [Header("Pause Menu UI Objects")]
    public GameObject PauseMenu;
    public CanvasGroup PauseMenuCanvasGroup;

    [Header("Options Menu UI Objects")]

    public GameObject OptionsMenu;

    [Header("Controls Menu UI Objects")]

    public GameObject ControlsMenu;

    [Header("Touch Controls UI")]
    public GameObject TouchControls;

    [Header("Blur Settings")]
    public Volume blurVolume; 
    public float fadeSpeed = 4f;
    
    private bool isPaused = false;
    private float targetWeight = 0f;
    private void Awake()
    {
        ResumeButton.onClick.AddListener(() =>
        {
            // Click
            HahaluGameManager.Instance.TogglePauseMenu();
        });
        QuitButton.onClick.AddListener(() =>
        {
            // Click
            Loader.Load(Loader.Scene.MainMenuScene);
        });
    }

    private void Start()
    {
        HahaluGameManager.Instance.OnGamePaused += HahaluGameManager_OnGamePaused;
        HahaluGameManager.Instance.OnGameUnPaused += HahaluGameManager_OnGameUnPaused;

        if (blurVolume != null) blurVolume.weight = 0f;
        if (PauseMenuCanvasGroup != null) PauseMenuCanvasGroup.alpha = 0f;
        Hide();
        HideAll();
    }
    private void Update()
    {
        if (blurVolume != null && blurVolume.weight != targetWeight)
        {
            float currentWeight = Mathf.MoveTowards(blurVolume.weight, targetWeight, fadeSpeed * Time.unscaledDeltaTime);
            
            blurVolume.weight = currentWeight;
            
            if (PauseMenuCanvasGroup != null)
            {
                PauseMenuCanvasGroup.alpha = currentWeight;
            }

            if (blurVolume.weight == 0f && !isPaused)
            {
                Hide();
            }
        }
    }

    private void HahaluGameManager_OnGameUnPaused(object sender, EventArgs e)
    {
        isPaused = false;
        targetWeight = 0f;
        HideAll();
    }

    private void HahaluGameManager_OnGamePaused(object sender, EventArgs e)
    {
        isPaused = true;
        targetWeight = 1f;
        Show();
    }

    private void Show()
    {
        if (TouchControls != null)
        {
            TouchControls.SetActive(false);
        }
        PauseMenu.SetActive(true);
        firstSelectedButtonPause.FocusMenu();
    }

    private void Hide()
    {
        PauseMenu.SetActive(false);
        Time.timeScale = 1f;
        if (TouchControls != null)
        {
            TouchControls.SetActive(true);
        }
    }

    private void HideAll()
    {
        OptionsMenu.SetActive(false);
        ControlsMenu.SetActive(false);

    }

    public void OnControlsButtonPressed()
    {
        PauseMenu.SetActive(false);
        ControlsMenu.SetActive(true);
        ControlsFirstSelectedButton.FocusMenu();
    }
    public void OnControlsBackButtonPressed()
    {
        ControlsMenu.SetActive(false);
        PauseMenu.SetActive(true);
        firstSelectedButtonPause.FocusMenu();
    }

    private void OnDestroy()
    {
        if (HahaluGameManager.Instance != null)
        {
        HahaluGameManager.Instance.OnGamePaused -= HahaluGameManager_OnGamePaused;
        HahaluGameManager.Instance.OnGameUnPaused -= HahaluGameManager_OnGameUnPaused;

        }
    }
}
