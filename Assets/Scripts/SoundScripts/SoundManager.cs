using System;
using Unity.VisualScripting;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set;}

    [SerializeField] private Camera MainCam;
    [SerializeField] private SoundClipsSO soundClipsSO;
    [SerializeField] private AudioSource playerWalk;
    [SerializeField] private AudioSource ambienceSound;

    private float volume = .1f;
    private void Awake()
    {
        Instance = this;
        ChangeInVolume();
    }

    private void Start()
    {
        DeliveryCounter.OnCorrectItemDelivery += DeliveryCounter_OnCorrectItemDelivery;
        DeliveryCounter.OnWrongItemDelivery += DeliveryCounter_OnWrongItemDelivery;
    }

    private void DeliveryCounter_OnWrongItemDelivery(object sender, EventArgs e)
    {
        PlaySound(soundClipsSO.deliveryFail, MainCam.transform.position);
    }

    private void DeliveryCounter_OnCorrectItemDelivery(object sender, EventArgs e)
    {
        //PlaySound(soundClipsSO.deliverySuccess, Camera.main.transform.position);
    }



    private void PlaySound(AudioClip[] audioClipArray, Vector3 position, float volume = 1f)
    {
        PlaySound(audioClipArray[UnityEngine.Random.Range(0, audioClipArray.Length)], position, volume);
    }
    private void PlaySound(AudioClip audioClip, Vector3 position, float volumeMultiplier = 1f)
    {
        AudioSource.PlayClipAtPoint(audioClip, position, volumeMultiplier * volume);
    }

    private void WalkSound()
    {
        if (playerWalk != null){
        playerWalk.volume = volume;
        }
    }

    private void AmbienceSound()
    {
        if (ambienceSound != null){
            ambienceSound.volume = volume;
        }
    }

    public void ChangeVolumeUP()
    {
        volume += .1f;
        if (volume > 1f)
        {
            volume = 1f;
        }
        ChangeInVolume();
    }
    public void ChangeVolumeDOWN()
    {
        volume -= .1f; 
        if (volume < 0)
        {
            volume = 0f;
        }
        ChangeInVolume();
    }

    // When you change Sound Effects Volume,
    // it changes other sound effects thats not played from this script
    private void ChangeInVolume()
    {
        WalkSound();
        AmbienceSound();
    }

    public float GetVolume()
    {
        return volume;
    }
}
