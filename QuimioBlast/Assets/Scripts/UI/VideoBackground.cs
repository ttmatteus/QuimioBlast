using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

[RequireComponent(typeof(RawImage))]
[RequireComponent(typeof(VideoPlayer))]
public class VideoBackground : MonoBehaviour
{
    private RenderTexture rt;

    private void Awake()
    {
        var rawImage = GetComponent<RawImage>();
        var player = GetComponent<VideoPlayer>();

        rt = new RenderTexture(1920, 1080, 0);
        player.targetTexture = rt;
        rawImage.texture = rt;

        player.isLooping = true;
        player.renderMode = VideoRenderMode.RenderTexture;
        player.audioOutputMode = VideoAudioOutputMode.None;
        player.Play();
    }

    private void OnDestroy()
    {
        if (rt != null) rt.Release();
    }
}
