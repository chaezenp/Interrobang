using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenuManager : MonoBehaviour
{

    [SerializeField] private Button MusicButtonUP;
    [SerializeField] private Button MusicButtonDOWN;
    [SerializeField] private Button SFXButtonUP;
    [SerializeField] private Button SFXButtonDOWN;

    [SerializeField] private TextMeshProUGUI musicText;
    [SerializeField] private TextMeshProUGUI SFXtext;

    private void Awake()
    {
        SFXButtonUP.onClick.AddListener(() =>
        {
            SoundManager.Instance.ChangeVolumeUP();
            UpdateVisual();
        } );
        SFXButtonDOWN.onClick.AddListener(() =>
        {
            SoundManager.Instance.ChangeVolumeDOWN();
            UpdateVisual();
        });
        MusicButtonUP.onClick.AddListener(() =>
        {
            MusicManager.Instance.ChangeVolumeUP();
            UpdateVisual();
        } );
        MusicButtonDOWN.onClick.AddListener(() =>
        {
            MusicManager.Instance.ChangeVolumeDOWN();
            UpdateVisual();
        });
    }

    private void Start()
    {
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        musicText.text = "Music: " + Mathf.Round(MusicManager.Instance.GetVolume()*10f);
        SFXtext.text = "Sound Effects: " + Mathf.Round(SoundManager.Instance.GetVolume()*10f);
    }

}
