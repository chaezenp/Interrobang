using UnityEngine;

public class MusicManager : MonoBehaviour
{

    public static MusicManager Instance { get; private set;}
    private AudioSource audioSource;
    private float volume = .1f;

    private void Awake()
    {
        Instance = this;
        audioSource = GetComponent<AudioSource>();
    }
    public void ChangeVolumeUP()
    {
        volume += .1f;
        audioSource.volume = volume;
        if (volume > 1f)
        {
            volume = 1f;
        }
    }
    public void ChangeVolumeDOWN()
    {
        volume -= .1f; 
        audioSource.volume = volume;
        if (volume < 0)
        {
            volume = 0f;
        }
    }

    public float GetVolume()
    {
        return volume;
    }
}
