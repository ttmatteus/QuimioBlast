using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

/// <summary>
/// Toca uma cinemática em vídeo antes de exibir o menu principal.
///
/// Como configurar na cena MenuPrincipal:
///   1. Crie um GameObject vazio "CinematicaIntro" na Hierarquia.
///   2. Adicione este script a ele.
///   3. Arraste o arquivo cinematica2.mp4 (VideoClip) para o campo "Video Clip".
///   4. Arraste o GameObject raiz do Canvas do menu para "Menu Canvas".
///      (O canvas do menu deve começar ATIVO na Hierarquia — este script o esconde
///       até o vídeo terminar.)
///   O vídeo não pode ser pulado — toca até o fim automaticamente.
/// </summary>
public class CinematicaIntro : MonoBehaviour
{
    [Tooltip("Arquivo de vídeo a tocar (VideoClip importado no projeto).")]
    [SerializeField] private VideoClip videoClip;

    [Tooltip("GameObject raiz do Canvas do menu. Será ocultado durante o vídeo.")]
    [SerializeField] private GameObject menuCanvas;

    private bool finished;

    private void Start()
    {
        if (menuCanvas != null)
            menuCanvas.SetActive(false);

        StartCoroutine(PlayCinematic());
    }

    private IEnumerator PlayCinematic()
    {
        if (videoClip == null)
        {
            ShowMenu();
            yield break;
        }

        // Canvas do vídeo (sortingOrder abaixo do SceneTransition que fica em 100)
        var canvasGO = new GameObject("CinematicCanvas");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 90;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        // Fundo preto
        var bgGO = new GameObject("BG");
        bgGO.transform.SetParent(canvasGO.transform, false);
        var bgRT = bgGO.AddComponent<RectTransform>();
        bgRT.anchorMin = Vector2.zero;
        bgRT.anchorMax = Vector2.one;
        bgRT.sizeDelta = Vector2.zero;
        bgGO.AddComponent<Image>().color = Color.black;

        // RenderTexture + RawImage
        var renderTex = new RenderTexture(1920, 1080, 0);
        var imgGO = new GameObject("VideoFrame");
        imgGO.transform.SetParent(canvasGO.transform, false);
        var imgRT = imgGO.AddComponent<RectTransform>();
        imgRT.anchorMin = Vector2.zero;
        imgRT.anchorMax = Vector2.one;
        imgRT.sizeDelta = Vector2.zero;
        imgGO.AddComponent<RawImage>().texture = renderTex;

        // VideoPlayer
        var vp = canvasGO.AddComponent<VideoPlayer>();
        vp.clip = videoClip;
        vp.targetTexture = renderTex;
        vp.renderMode = VideoRenderMode.RenderTexture;
        vp.audioOutputMode = VideoAudioOutputMode.Direct;
        vp.isLooping = false;
        vp.playOnAwake = false;
        vp.loopPointReached += _ => finished = true;

        // Prepara e toca
        vp.Prepare();
        while (!vp.isPrepared)
            yield return null;

        vp.Play();

        while (!finished)
            yield return null;

        vp.Stop();
        renderTex.Release();
        Destroy(canvasGO);
        ShowMenu();
    }

    private void ShowMenu()
    {
        if (menuCanvas != null)
            menuCanvas.SetActive(true);
    }
}
