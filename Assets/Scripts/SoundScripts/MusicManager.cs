using UnityEngine;

public class MusicManager : MonoBehaviour
{

    public static MusicManager Instance { get; private set;}
    private AudioSource audioSource;
    private float volume = .1f;

    // Saves Music Volume to device
    private const string MUSIC_VOLUME_KEY = "MusicVolumeSetting";

    private void Awake()
    {
        Instance = this;
        audioSource = GetComponent<AudioSource>();
        
        volume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 0.1f);
    }

    private void Start()
    {
        audioSource.volume = volume;
    }
    public void ChangeVolumeUP()
    {
        volume += .1f;
        if (volume > 1f)
        {
            volume = 1f;
        }
        
        SaveAndApply();
    }

    public void ChangeVolumeDOWN()
    {
        volume -= .1f; 
        if (volume < 0f)
        {
            volume = 0f;
        }

        SaveAndApply();
    }

    private void SaveAndApply()
    {
        audioSource.volume = volume;

        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, volume);
        PlayerPrefs.Save(); 
    }

    public float GetVolume()
    {
        return volume;
    }
}
