using UnityEngine;
using UnityEngine.Video;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.IO;

public class PauseTutorialSlidesClick : MonoBehaviour
{
    [Header("UI Display")]
    [SerializeField] private RawImage uiDisplayImage; 

    [Header("Video Players")]
    [SerializeField] private VideoPlayer playerA;
    [SerializeField] private VideoPlayer playerB;

    [Header("Playlist (StreamingAssets)")]
    [Tooltip("Enter the file names exactly as they appear in StreamingAssets")]
    [SerializeField] private string[] playlistFileNames;

    [SerializeField] private InputActionReference navigateActionReference;

    private VideoPlayer activePlayer;
    private VideoPlayer backgroundPlayer;
    private int currentVideoIndex = 0;
    private RenderTexture textureA;
    private RenderTexture textureB;

    private bool isPreparingNavigation = false;

    private void OnEnable()
    {
        if (navigateActionReference != null && navigateActionReference.action != null)
        {
            navigateActionReference.action.performed += OnNavigateInput;
            navigateActionReference.action.Enable();
        }

        playerA.loopPointReached += OnVideoLoopPointReached;
        playerB.loopPointReached += OnVideoLoopPointReached;

        playerA.prepareCompleted += OnNavigationPrepareCompleted;
        playerB.prepareCompleted += OnNavigationPrepareCompleted;

        InitializeVideoPlaylist();
    }

    private void OnDisable()
    {
        if (navigateActionReference != null && navigateActionReference.action != null)
        {
            navigateActionReference.action.performed -= OnNavigateInput;
        }

        // Unsubscribe safely
        playerA.loopPointReached -= OnVideoLoopPointReached;
        playerB.loopPointReached -= OnVideoLoopPointReached;
        playerA.prepareCompleted -= OnNavigationPrepareCompleted;
        playerB.prepareCompleted -= OnNavigationPrepareCompleted;

        if (activePlayer != null) activePlayer.Stop();
        if (backgroundPlayer != null) backgroundPlayer.Stop();
    }

    private void InitializeVideoPlaylist()
    {
        playerA.source = VideoSource.Url;
        playerB.source = VideoSource.Url;

        playerA.isLooping = false;
        playerB.isLooping = false;

        if (playlistFileNames == null || playlistFileNames.Length == 0)
        {
            Debug.LogWarning("Playlist is empty!");
            return;
        }

        if (uiDisplayImage == null)
        {
            Debug.LogError("Assign a RawImage component!");
            return;
        }

        if (textureA == null) textureA = new RenderTexture(1920, 1080, 16, RenderTextureFormat.ARGB32);
        if (textureB == null) textureB = new RenderTexture(1920, 1080, 16, RenderTextureFormat.ARGB32);
        
        if (!textureA.IsCreated()) textureA.Create();
        if (!textureB.IsCreated()) textureB.Create();

        playerA.renderMode = VideoRenderMode.RenderTexture;
        playerA.targetTexture = textureA;

        playerB.renderMode = VideoRenderMode.RenderTexture;
        playerB.targetTexture = textureB;

        currentVideoIndex = 0;
        isPreparingNavigation = false;
        activePlayer = playerA;
        backgroundPlayer = playerB;

        uiDisplayImage.texture = textureA;

        activePlayer.url = GetStreamingAssetsUrl(playlistFileNames[0]);
        activePlayer.Play();

        PrepareBackgroundAsLoopBuffer();
    }

    private void OnVideoLoopPointReached(VideoPlayer source)
    {
        // Only trigger an automatic loop swap if we aren't currently waiting for a new slide to load
        if (!isPreparingNavigation)
        {
            ExecuteHardwareTextureSwap();
        }
        else
        {
            // If the user hit next/back and the current video finished before the new video was ready,
            // we restart the current video to hold the screen state smoothly so it never drops to black
            activePlayer.Play();
        }
    }

    private void PrepareBackgroundAsLoopBuffer()
    {
        if (currentVideoIndex < playlistFileNames.Length && !string.IsNullOrEmpty(playlistFileNames[currentVideoIndex]))
        {
            backgroundPlayer.url = GetStreamingAssetsUrl(playlistFileNames[currentVideoIndex]);
            backgroundPlayer.Prepare();
        }
    }

    private void AdvancePlaylist()
    {
        if (currentVideoIndex >= playlistFileNames.Length - 1 || isPreparingNavigation) return;

        currentVideoIndex++;
        InitiateSlideChange();
    }

    private void RewindPlaylist()
    {
        if (currentVideoIndex <= 0 || isPreparingNavigation) return;

        currentVideoIndex--;
        InitiateSlideChange();
    }

    private void InitiateSlideChange()
    {
        // Set lock state so the active video keeps looping and ignores standard loop swaps
        isPreparingNavigation = true;

        // Change the background URL to our destination
        backgroundPlayer.url = GetStreamingAssetsUrl(playlistFileNames[currentVideoIndex]);
        backgroundPlayer.Prepare(); 
    }

    private void OnNavigationPrepareCompleted(VideoPlayer source)
    {
        if (isPreparingNavigation && source == backgroundPlayer)
        {
            isPreparingNavigation = false;
            ExecuteHardwareTextureSwap();
        }
    }

    private void ExecuteHardwareTextureSwap()
    {
        backgroundPlayer.Play();
        activePlayer.Stop();

        VideoPlayer temp = activePlayer;
        activePlayer = backgroundPlayer;
        backgroundPlayer = temp;

        uiDisplayImage.texture = activePlayer.targetTexture;

        PrepareBackgroundAsLoopBuffer();
    }

    private void OnNavigateInput(InputAction.CallbackContext context)
    {
        float inputAxis = 0f;
        if (context.valueType == typeof(Vector2))
        {
            inputAxis = context.ReadValue<Vector2>().x;
        }
        else
        {
            inputAxis = context.ReadValue<float>();
        }

        if (inputAxis > 0.1f) AdvancePlaylist();
        else if (inputAxis < -0.1f) RewindPlaylist();
    }

    public void OnNextButtonClick() => AdvancePlaylist();
    public void OnBackButtonClick() => RewindPlaylist();

    private string GetStreamingAssetsUrl(string fileName)
    {
        string combinedPath = Path.Combine(Application.streamingAssetsPath, fileName);
        return combinedPath.Replace("\\", "/");
    }

    private void OnDestroy()
    {
        if (textureA != null) textureA.Release();
        if (textureB != null) textureB.Release();
    }
}
