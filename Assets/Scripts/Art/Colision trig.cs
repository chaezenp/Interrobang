using UnityEngine;

public class AnimationTrigger : MonoBehaviour

{
    
    [SerializeField] private Animator objectAnimator;

    [SerializeField] private string animationBoolName = "Parameter";


    private void OnTriggerEnter(Collider other)

    {
        
        if (other.CompareTag("Player"))
        {
            SetAnimationState(true);
        }

    }
    
    private void OnTriggerExit(Collider other)

    {
        if (other.CompareTag("Player"))

        {
            SetAnimationState(false);
        }
    }


    private void SetAnimationState(bool state)

    {
        if (objectAnimator != null)
        {
            objectAnimator.SetBool(animationBoolName, state);
        }
    }
} // writen by ye boi JV