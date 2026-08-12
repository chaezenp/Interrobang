using UnityEngine;
using UnityEngine.UI;

public class NPCsliderContols : MonoBehaviour
{
    [SerializeField] private Animator targetAnimator;

    [SerializeField] private Slider progressSlider;

    [SerializeField] private string parameterName = "parameter";
    [SerializeField] private string parameterBool = "isChasing";
    private bool isChasingAnim = false;

    private void Update()
    {
        if (targetAnimator == null) return;
        
        if (progressSlider != null)
        {
          
            targetAnimator.SetFloat(parameterName, progressSlider.value);

        }

        if (isChasingAnim)
        {
            targetAnimator.SetBool(parameterBool, true);
        }
        else
        {
            targetAnimator.SetBool(parameterBool, false);
        }
                Debug.Log("apples and " + isChasingAnim);


    }

    public void isChasingPlayer(bool isChasing)
    {
        isChasingAnim = isChasing;
    }
}
// writen by JV :P
//script to set anim float to the same as slider value.. note to self control triggers viya animator float values