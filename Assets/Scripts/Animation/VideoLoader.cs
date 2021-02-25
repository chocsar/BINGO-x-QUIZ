using UnityEngine;
using UnityEngine.Video;
[RequireComponent(typeof(VideoPlayer))]
public class VideoLoader : MonoBehaviour
{
    [SerializeField] string url;
    void Start()
    {

        var videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = url;
        videoPlayer.prepareCompleted += PrepareCompleted;
        videoPlayer.Prepare();
    }
    void PrepareCompleted(VideoPlayer vp)
    {
        vp.prepareCompleted -= PrepareCompleted;
        vp.Play();
    }
}