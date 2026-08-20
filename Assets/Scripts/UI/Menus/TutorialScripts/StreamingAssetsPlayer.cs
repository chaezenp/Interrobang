using UnityEngine;
using UnityEngine.Video;
using System.IO;

[RequireComponent(typeof(VideoPlayer))]
public class StreamingVideoPlayer : MonoBehaviour
{
    [SerializeField] private string videoFileName; 

    private VideoPlayer videoPlayer;

    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();

        videoPlayer.source = VideoSource.Url;

        string absolutePath = Path.Combine(Application.streamingAssetsPath, videoFileName);

        videoPlayer.url = absolutePath;

        videoPlayer.prepareCompleted += OnVideoPrepared;
        videoPlayer.Prepare();
    }

    void OnVideoPrepared(VideoPlayer vp)
    {
        vp.Play();
    }
}
