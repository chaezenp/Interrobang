using UnityEngine;
using UnityEngine.InputSystem;
public class ToggleMobileUI : MonoBehaviour
{
    [Header("Pause Menu Ref")]
    public PauseMenuUI pauseMenuUI;
    [Header("Mobile UI Configuration")]
    [SerializeField] private GameObject mobileUiPanel;
    
    // Set to true if you want to make UI visible while testing in the Unity Editor
    [SerializeField] private bool forceShowInEditor = true;

    void Start()
    {
        if (mobileUiPanel == null)
        {
            Debug.LogWarning("Mobile UI Panel reference missing from ToggleMobileUI script.", this);
            return;
        }

        #if UNITY_EDITOR
        if (forceShowInEditor)
        {
            mobileUiPanel.SetActive(true);
            return;
        }
        #endif

        if (Application.isMobilePlatform)
        {
            // Device has a touch screen
            mobileUiPanel.SetActive(true);
            pauseMenuUI.TouchControls = mobileUiPanel;
        }
        else
        {
            // Device lacks a touch screen
            mobileUiPanel.SetActive(false);
        }
    }
}
