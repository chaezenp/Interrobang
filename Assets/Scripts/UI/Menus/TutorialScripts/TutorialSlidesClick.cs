using UnityEngine;
using UnityEngine.Video;

public class TutorialSlidesClick : MonoBehaviour 
{ 
    [Header("Video Players")] 
    [SerializeField] private VideoPlayer playerA; 
    [SerializeField] private VideoPlayer playerB; 

    [Header("Playlist Settings")] 
    [SerializeField] private VideoClip[] playlist; 

    private VideoPlayer activePlayer; 
    private VideoPlayer backgroundPlayer; 
    private int currentVideoIndex = 0; 
    private bool isChangingScene = false; 

    void Start() 
    { 
        if (playlist == null || playlist.Length == 0) 
        { 
            LoadNextScene(); 
            return; 
        } 

        activePlayer = playerA; 
        backgroundPlayer = playerB; 

        activePlayer.clip = playlist[0]; 
        activePlayer.Play(); 

        PrepareNextVideo(); 
    } 
    public void OnNextButtonClick() 
    { 
        if (isChangingScene) return; 
        AdvancePlaylist(); 
    } 
    public void OnBackButtonClick()
    {
        if (isChangingScene || currentVideoIndex <= 0) return;

        RewindPlaylist();
    }

    private void AdvancePlaylist() 
    { 
        if (currentVideoIndex >= playlist.Length - 1) 
        { 
            LoadNextScene(); 
        } 
        else 
        { 
            SwapPlayersForward(); 
        } 
    } 

    private void RewindPlaylist()
    {
        currentVideoIndex--;

        backgroundPlayer.clip = playlist[currentVideoIndex];
        backgroundPlayer.Play();
        
        activePlayer.Stop();

        VideoPlayer temp = activePlayer; 
        activePlayer = backgroundPlayer; 
        backgroundPlayer = temp; 

        PrepareNextVideo();
    }

    private void PrepareNextVideo() 
    { 
        int nextIndex = currentVideoIndex + 1; 
        if (nextIndex < playlist.Length && playlist[nextIndex] != null) 
        { 
            backgroundPlayer.clip = playlist[nextIndex]; 
            backgroundPlayer.Prepare(); 
        } 
    } 

    private void SwapPlayersForward() 
    { 
        currentVideoIndex++; 

        backgroundPlayer.Play(); 
        activePlayer.Stop(); 

        VideoPlayer temp = activePlayer; 
        activePlayer = backgroundPlayer; 
        backgroundPlayer = temp; 

        PrepareNextVideo(); 
    } 

    private void LoadNextScene() 
    { 
        isChangingScene = true; 
        if (activePlayer != null) activePlayer.Stop(); 
        if (backgroundPlayer != null) backgroundPlayer.Stop(); 

        Loader.Load(Loader.Scene.TestLevelWithART); 
    } 
}
