using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ChopInputUI : MonoBehaviour
{
    [SerializeField] private BaseCounter baseCounter;
    [SerializeField] private BaseCounter currentCounter;
    
    [Header("Tap UI Prompts")]
    [SerializeField] private GameObject TapButtonKeyboard;
    [SerializeField] private GameObject TapButtonGamepad;
        
    [Header("Tap x3 UI Prompts")]
    [SerializeField] private GameObject TapButton3;
    [SerializeField] private TextMeshProUGUI TapNumber;

    private enum LayoutType { Keyboard, GamepadANDMobile }
    private LayoutType currentLayoutType = LayoutType.Keyboard; 
    private bool isPlayerNearby = false;
    private bool isTap3= true;
    private bool isTap1 = true;
    private int pressesNeeded = 3;
    private bool itemonCounter = false;

    private void Start()
    {
        PlayerController.Instance.OnSelectedCounterChanged += OnSelectedCounterChanged;
        InputSystem.onActionChange += OnActionControlsChanged;
        if (baseCounter is CuttingCounter cuttingCounter)
        {
            cuttingCounter.OnItemPlaced += CuttingCounter_OnItemPlaced;
        }
    }

    private void CuttingCounter_OnItemPlaced(object sender, EventArgs e)
    {
        UpdateVisualState();
    }

    private void OnDestroy()
    {
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.OnSelectedCounterChanged -= OnSelectedCounterChanged;
        }
        InputSystem.onActionChange -= OnActionControlsChanged;
    }

    private void OnActionControlsChanged(object obj, InputActionChange change)
    {
        if (change == InputActionChange.ActionStarted && obj is InputAction action)
        {
            InputDevice lastDevice = action.activeControl?.device;

            if (lastDevice is Keyboard || lastDevice is Mouse)
            {
                UpdateLayoutType(LayoutType.Keyboard);
            }
            else if (lastDevice is Gamepad || lastDevice is Touchscreen || lastDevice.name.Contains("OnScreen"))
            {
                UpdateLayoutType(LayoutType.GamepadANDMobile);
            }
        }
    }

    private void UpdateLayoutType(LayoutType newLayout)
    {
        if (currentLayoutType == newLayout) return; 

        currentLayoutType = newLayout;

        if (isPlayerNearby)
        {
            RefreshUI();
        }
    }

    private void OnSelectedCounterChanged(object sender, PlayerController.OnSelectedCounterChangedEventArgs e)
    {
        currentCounter = e.selectedCounter;
        UpdateVisualState();
    }

    private void UpdateVisualState()
    {
        itemonCounter = baseCounter.HasItemObject();

        if (currentCounter == baseCounter && itemonCounter)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    private void Show()
    {
        isPlayerNearby = true;
        RefreshUI();
    }

    public void Hide()
    {
        isPlayerNearby = false;
        
        // Disable all layouts when player walks away
        TapButtonKeyboard.SetActive(false);
        TapButtonGamepad.SetActive(false);
        TapButton3.SetActive(false);
    }

    private void RefreshUI()
    {
        bool isKeyboard = (currentLayoutType == LayoutType.Keyboard);
        bool isGamepadANDMobile = (currentLayoutType == LayoutType.GamepadANDMobile);

        if (isTap1)
        {
            TapButtonKeyboard.SetActive(isKeyboard);
            TapButtonGamepad.SetActive(isGamepadANDMobile);
        }
        else
        {
            TapButtonKeyboard.SetActive(false);
            TapButtonGamepad.SetActive(false);
        }

        if (isTap3)
        {
            TapButton3.SetActive(true); 
        }
        else
        {
            TapButton3.SetActive(false);
        }
    }

    public void ChangeTapNumber(int numberPressedAlready)
    {
        if (pressesNeeded > 0){
        pressesNeeded = 3 - numberPressedAlready;

        TapNumber.text = "    x" + pressesNeeded;
        }

    }

    public void ResetBools()
    {
        pressesNeeded = 3;
        TapNumber.text = "    x" + pressesNeeded;
    }
}
