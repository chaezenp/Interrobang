using UnityEngine;
using UnityEngine.InputSystem;


public class TargetSpecificAnimator : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Animator TargetAnimator;
    [SerializeField] private string ParameterName = "Parameter";

    // new part added / testing for holding iteam animation
    
    private PlayerController targetComponent; 

    void Start()
    {
        targetComponent = GetComponent<PlayerController>();
    }
  

    void Update()
    {
        
        if (TargetAnimator == null) return;

        bool isKeyboardMoving = Keyboard.current != null &&
            
            (
             Keyboard.current.wKey.isPressed ||
             Keyboard.current.aKey.isPressed ||
             Keyboard.current.sKey.isPressed ||
             Keyboard.current.dKey.isPressed
            );


// for who ever is reading dis the section bellow is for the xbox controler imputs. also note to self i need to change
//the sprint button on controller. talk with team on that :P

        bool isControllerMoving = false;
        if(Gamepad.current != null)
        {
            Vector2 stickInput = Gamepad.current.leftStick.ReadValue();
            if(stickInput.magnitude  > 0.1f)

            {
                isControllerMoving = true;
            }
        }
        bool isMoving = isKeyboardMoving || isControllerMoving;

        bool isKeyboardSprinting = Keyboard.current != null &&
                               (Keyboard.current.leftShiftKey.isPressed ||
                                Keyboard.current.rightShiftKey.isPressed);

        bool isControllerSprinting = Gamepad.current != null &&
                               (Gamepad.current.leftStickButton.isPressed ||
                                Gamepad.current.buttonWest.isPressed );
                                

 // bellow is value controls and combining inputs for teh controller
     
        bool isSprinting = isKeyboardSprinting || isControllerSprinting;

        float moveValue = 0f;

     
     //jogging
        if (isMoving && isSprinting)
            {
                moveValue = 1.0f;
            }
      
      //walking
        else if (isMoving)
            {
                moveValue = 0.5f;
            }
     
     //idle
        else
            {
                moveValue = 0f;
            }
    // more exsperimenting and testing

        if (targetComponent != null && targetComponent.HasItemObject() == true)
        {
            moveValue += 2.0f;
        }

        TargetAnimator.SetFloat(ParameterName, moveValue);

        // playercontroller.hasItemObject()  is the correct coponent

        }
    }

//note to self bellow

//https://www.youtube.com/watch?v=5mlwvbu1fxQ credit to creator --> RSDevelopment

//above is the video used to help make setup. Note had to convert old method of movment --> GetKey(KeyCod. Example key) to newer 
// methoud----> (Keyboard.current.example Key.isPressed) note made a bunch of changes compaired to the videos for our use case
// this is my own code not a copy and not generated.

// disclamer I used gemini to do some debugging and finding spelling errors all of this is still hand typed put together by me JV ;P