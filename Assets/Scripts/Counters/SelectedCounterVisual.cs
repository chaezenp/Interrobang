using System;
using UnityEngine;

public class SelectedCounterVisual : MonoBehaviour
{

    [SerializeField] private BaseCounter baseCounter;
    //[SerializeField] private GameObject[] visualGameObjectArray;
    [SerializeField] private Animator objectAnimator;

    [SerializeField] private string animationBoolName = "Parameter";

    private void Start()
    {
        PlayerController.Instance.OnSelectedCounterChanged += OnSelectedCounterChanged;
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
        // foreach (GameObject visualGameObject in visualGameObjectArray)
        // {
        //     visualGameObject.SetActive(true);
        // }
        SetAnimationState(true);

    }

    private void Hide()
    {
        // foreach (GameObject visualGameObject in visualGameObjectArray)
        // {
        //     visualGameObject.SetActive(false);
        // }
        SetAnimationState(false);

    }

    private void SetAnimationState(bool state)

    {
        if (objectAnimator != null)
        {
            objectAnimator.SetBool(animationBoolName, state);
        }
    }
}
