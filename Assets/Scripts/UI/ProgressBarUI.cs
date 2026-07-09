using System;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUI : MonoBehaviour
{
    [SerializeField] private FillStation fillStation;
    [SerializeField] private Image barImage;

    private void Start()
    {
        fillStation.OnProgressChanged += FillStation_OnProgressChanged;
        barImage.fillAmount = 0f;
        Hide();
    }

    private void FillStation_OnProgressChanged(object sender, FillStation.OnProgressChangedEventArgs e)
    {
        barImage.fillAmount = e.progressNormalized;

        if (e.progressNormalized == 0f || e.progressNormalized == 1f)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

}
