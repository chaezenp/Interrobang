using UnityEngine;
using UnityEngine.UI;

public class NPCsliderContols : MonoBehaviour
{

    [SerializeField] private Animator targetAnimator;

    [SerializeField] private Slider progressSlider;

    [SerializeField] private string parameterName = "parameter";

    private void Update()
    {
        if (targetAnimator != null && progressSlider != null)
        {
          
            targetAnimator.SetFloat(parameterName, progressSlider.value);

        }
    }
}
// writen by JV :P
//script to set anim float to the same as slider value.. note to self control triggers viya animator float values