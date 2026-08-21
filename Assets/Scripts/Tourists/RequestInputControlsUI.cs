using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class RequestInputControlsUI : MonoBehaviour
{
    [SerializeField] private BaseCounter baseCounter;
    
    [Header("Tap UI Prompts")]
    [SerializeField] private GameObject TapButtonKeyboard;
    [SerializeField] private GameObject TapButtonGamepad;
        
    [Header("Tap x3 UI Prompts")]
    [SerializeField] private GameObject TapButton3;
    [SerializeField] private TextMeshProUGUI TapNumber;

    [Header("Hold UI Prompts")]
    [SerializeField] private GameObject HoldButtonKeyboard;
    [SerializeField] private GameObject HoldButtonGamepad;

    private enum LayoutType { Keyboard, GamepadANDMobile }
    private LayoutType currentLayoutType = LayoutType.Keyboard; 
    private bool isPlayerNearby = false;
    private string itemName;
    private bool isTap3= false;
    private bool isTap1 = false;
    private bool isHold = false;
    private int pressesNeeded = 3;

    private void Start()
    {
        PlayerController.Instance.OnSelectedCounterChanged += OnSelectedCounterChanged;
        InputSystem.onActionChange += OnActionControlsChanged;
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
        if (e.selectedCounter == baseCounter)
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
        HoldButtonKeyboard.SetActive(false);
        HoldButtonGamepad.SetActive(false);
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

        if (isHold)
        {
            HoldButtonKeyboard.SetActive(isKeyboard);
            HoldButtonGamepad.SetActive(isGamepadANDMobile);
        }
        else
        {
            HoldButtonKeyboard.SetActive(false);
            HoldButtonGamepad.SetActive(false);
        }
    }

    public void TellUIWhatItem(string theItemName)
    {
        itemName = theItemName;
        switch (itemName)
        {
            case "Sunscreen":
                isTap1 = true;
                isTap3 = true;
                break;
            case "CoconutDrinkFilled":
                isTap1 = true;
                break;
            case "Towel":
                isHold = true;
                break;
            case "FullPokeBowl":
                isTap1 = true;
                break;
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
        isTap1 = false;
        isHold = false;
        isTap3 = false;
        pressesNeeded = 3;
    }
}
