using UnityEngine;
using UnityEngine.Video;

public class TutorialSlidesTimer : MonoBehaviour
{
    [Header("Video Players")]
    [SerializeField] private VideoPlayer playerA;
    [SerializeField] private VideoPlayer playerB;

    [Header("Playlist Settings")]
    [SerializeField] private VideoClip[] playlist;
    [SerializeField] private float changeInterval = 10f;

    private VideoPlayer activePlayer;
    private VideoPlayer backgroundPlayer;
    
    private float timer = 0f;
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

    void Update()
    {
        if (isChangingScene) return;

        timer += Time.deltaTime;

        if (timer >= changeInterval)
        {
            timer = 0f;
            
            if (currentVideoIndex >= playlist.Length - 1)
            {
                LoadNextScene();
            }
            else
            {
                SwapPlayers();
            }
        }
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

    private void SwapPlayers()
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
        
        // Stop any running videos safely
        if (activePlayer != null) activePlayer.Stop();
        if (backgroundPlayer != null) backgroundPlayer.Stop();

        Loader.Load(Loader.Scene.TestLevelWithART);
    }
}

