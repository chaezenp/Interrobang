using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject MainMenuUIObj;
    [SerializeField] private CanvasGroup MainMenuCanvasGroup;
    [SerializeField] private GameObject CreditsUI;
    [SerializeField] private CanvasGroup CreditsCanvasGroup;
    [SerializeField] private GameObject OptionsUI;
    [SerializeField] private CanvasGroup OptionsCanvasGroup;
    [SerializeField] private FirstSelectedButton firstSelectedButtonMainMenu;
    [SerializeField] private Button playButton;
    [SerializeField] private Button OptionsButton;
    [SerializeField] private Button OptionsBackButton;
    [SerializeField] private Button QuitButton;
    [SerializeField] private Button CreditsButton;
    [SerializeField] private Button CreditsBackButton;
    [SerializeField] private Animator objectAnimator;

    [SerializeField] private string animationBoolName = "Parameter";

    [Header("Blur Settings")]
    public Volume blurVolume; 
    public float fadeSpeed = 0.15f;
    
    private float creditsTargetWeight = 1f;
    private float MMtargetWeight = 0f;
    private bool inCredits = false;
    private bool inOptions = false;

    private void Awake()
    {
        playButton.onClick.AddListener(() =>
        {
            // Click
            Loader.Load(Loader.Scene.TutorialSlidesScene);
        });
        OptionsButton.onClick.AddListener(() =>
        {
            // Click
            SetAnimationState(true);
            inOptions = true;
            Hide();
            ShowOptions();
            firstSelectedButtonMainMenu.FocusCreditsMenu(OptionsBackButton);
            
        });;
        OptionsBackButton.onClick.AddListener(() =>
        {
            // Click
            SetAnimationState(false);
            inOptions = false;
            HideOptions();
            Show();
            firstSelectedButtonMainMenu.FocusCreditsMenu(playButton);
            
        });;
        CreditsButton.onClick.AddListener(() =>
        {
            // Click
            SetAnimationState(true);
            inCredits = true;
            Hide();
            ShowCredits();
            firstSelectedButtonMainMenu.FocusCreditsMenu(CreditsBackButton);
        });
        CreditsBackButton.onClick.AddListener(() =>
        {
            // Click
            SetAnimationState(false);
            inCredits = false;
            HideCredits();
            Show();
            firstSelectedButtonMainMenu.FocusCreditsMenu(playButton);
        });
        QuitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });;
        Time.timeScale = 1f;
    }
    private void Start()
    {        
        if (blurVolume != null) blurVolume.weight = 0f;

        HideCredits();
        HideOptions();
        Show();
        
        firstSelectedButtonMainMenu.FocusMenu();
    }

    private void Update()
    {
        if (blurVolume != null && blurVolume.weight != creditsTargetWeight && inCredits)
        {            
            float currentWeight = Mathf.MoveTowards(blurVolume.weight, creditsTargetWeight, fadeSpeed * Time.unscaledDeltaTime);
            
            blurVolume.weight = currentWeight;
            if (CreditsCanvasGroup != null)
            {
                CreditsCanvasGroup.alpha = currentWeight;
            }
        }
        
        if (blurVolume != null && blurVolume.weight != creditsTargetWeight && inOptions)
        {            
            float currentWeight = Mathf.MoveTowards(blurVolume.weight, creditsTargetWeight, fadeSpeed * Time.unscaledDeltaTime);
            
            blurVolume.weight = currentWeight;
            if (OptionsCanvasGroup != null)
            {
                OptionsCanvasGroup.alpha = currentWeight;
            }
        }
        
        if (blurVolume != null && blurVolume.weight != MMtargetWeight && !inCredits && !inOptions)
        {
            float currentWeight = Mathf.MoveTowards(blurVolume.weight, MMtargetWeight, fadeSpeed * Time.unscaledDeltaTime);
            
            blurVolume.weight = currentWeight;
            if (MainMenuCanvasGroup != null)
            {
                MainMenuCanvasGroup.alpha = 1f - currentWeight;
            }
        }

    }
    private void Show()
    {
        MainMenuUIObj.SetActive(true);
    }
    private void ShowCredits()
    {
        CreditsUI.SetActive(true);
    }
    private void ShowOptions()
    {
        OptionsUI.SetActive(true);
    }

    private void Hide()
    {
        MainMenuUIObj.SetActive(false);
    }
    private void HideCredits()
    {
        CreditsUI.SetActive(false);
    }
    private void HideOptions()
    {
        OptionsUI.SetActive(false);
    }


    private void SetAnimationState(bool state)

    {
        if (objectAnimator != null)
        {
            objectAnimator.SetBool(animationBoolName, state);
        }
    }
}
