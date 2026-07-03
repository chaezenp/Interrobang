using UnityEngine;
using UnityEngine.InputSystem;

// disclamer I used gemini to do some debugging and finding spelling errors all of this is still hand typed put together by me ;P
public class TargetSpecificAnimator : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Animator TargetAnimator;
    [SerializeField] private string ParameterName = "Parameter";

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

     
     //jogging
        if (isMoving && isSprinting)
            {
                TargetAnimator.SetFloat(ParameterName, 1.0f);
            }
      
      //walking
        else if (isMoving)
            {
                TargetAnimator.SetFloat(ParameterName, 0.5f);
            }
     
     //idle
        else
            {
                TargetAnimator.SetFloat(ParameterName, 0f);
            }

        }
    }

//note to slef bellow

//https://www.youtube.com/watch?v=5mlwvbu1fxQ credit to creator --> RSDevelopment

//above is the video used to help make setup. Note had to convert old method of movment --> GetKey(KeyCod. Example key) to newer 
// methoud----> (Keyboard.current.example Key.isPressed) made a bunch of changes compaired to the videos for our use case