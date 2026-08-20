using UnityEngine;
using UnityEngine.Video;
using UnityEngine.InputSystem; // Required for Input Actions in Unity 6
using System.IO;

public class TutorialSlidesClick : MonoBehaviour
{
    [Header("Video Players")]
    [SerializeField] private VideoPlayer playerA;
    [SerializeField] private VideoPlayer playerB;

    [Header("Playlist Settings (StreamingAssets)")]
    [Tooltip("Enter the file names exactly as they appear in StreamingAssets, e.g., video1.mp4")]
    [SerializeField] private string[] playlistFileNames;

    [Header("Existing Input Map Configuration")]
    [SerializeField] private InputActionReference navigateActionReference;

    private VideoPlayer activePlayer;
    private VideoPlayer backgroundPlayer;
    private int currentVideoIndex = 0;
    private bool isChangingScene = false;

    private void OnEnable()
    {
        if (navigateActionReference != null && navigateActionReference.action != null)
        {
            navigateActionReference.action.performed += OnNavigateInput;
            navigateActionReference.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (navigateActionReference != null && navigateActionReference.action != null)
        {
            navigateActionReference.action.performed -= OnNavigateInput;
        }
    }

    void Start()
    {
        playerA.source = VideoSource.Url;
        playerB.source = VideoSource.Url;

        if (playlistFileNames == null || playlistFileNames.Length == 0)
        {
            LoadNextScene();
            return;
        }

        activePlayer = playerA;
        backgroundPlayer = playerB;

        activePlayer.url = GetStreamingAssetsUrl(playlistFileNames[0]);
        activePlayer.Play();

        PrepareNextVideo();
    }

    private void OnNavigateInput(InputAction.CallbackContext context)
    {
        if (isChangingScene) return;

        float inputAxis = 0f;
        if (context.valueType == typeof(Vector2))
        {
            inputAxis = context.ReadValue<Vector2>().x;
        }
        else
        {
            inputAxis = context.ReadValue<float>();
        }

        if (inputAxis > 0.1f)
        {
            AdvancePlaylist();
        }
        else if (inputAxis < -0.1f)
        {
            if (currentVideoIndex <= 0) return;
            RewindPlaylist();
        }
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
        if (currentVideoIndex >= playlistFileNames.Length - 1)
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
        
        // Load target video URL into background player and play it
        backgroundPlayer.url = GetStreamingAssetsUrl(playlistFileNames[currentVideoIndex]);
        backgroundPlayer.Play();
        
        activePlayer.Stop();

        // Swap players
        VideoPlayer temp = activePlayer;
        activePlayer = backgroundPlayer;
        backgroundPlayer = temp;

        PrepareNextVideo();
    }

    private void PrepareNextVideo()
    {
        int nextIndex = currentVideoIndex + 1;
        if (nextIndex < playlistFileNames.Length && !string.IsNullOrEmpty(playlistFileNames[nextIndex]))
        {
            backgroundPlayer.url = GetStreamingAssetsUrl(playlistFileNames[nextIndex]);
            backgroundPlayer.Prepare();
        }
    }

    private void SwapPlayersForward()
    {
        currentVideoIndex++;
        backgroundPlayer.Play();
        activePlayer.Stop();

        // Swap players
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
    private string GetStreamingAssetsUrl(string fileName)
    {
        string combinedPath = Path.Combine(Application.streamingAssetsPath, fileName);
        
        return combinedPath.Replace("\\", "/");
    }

}
