using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Singleton persistente que faz fade preto entre todas as trocas de cena.
/// Criado automaticamente no início do jogo — não precisa ser colocado em nenhuma cena.
/// Use SceneTransition.Instance.LoadScene("NomeDaCena") em vez de SceneManager.LoadScene().
/// </summary>
public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance { get; private set; }

    [SerializeField] private float fadeDuration = 0.5f;

    private Image overlay;
    private bool transitioning;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void AutoCreate()
    {
        var go = new GameObject("SceneTransition");
        go.AddComponent<SceneTransition>();
        DontDestroyOnLoad(go);
    }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        BuildOverlay();
        overlay.color = Color.black;
        StartCoroutine(FadeRoutine(1f, 0f));
    }

    private void BuildOverlay()
    {
        var canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        var scaler = gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        gameObject.AddComponent<GraphicRaycaster>();

        var imgGO = new GameObject("Overlay");
        imgGO.transform.SetParent(transform, false);

        var rt = imgGO.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;

        overlay = imgGO.AddComponent<Image>();
        overlay.color = Color.clear;
        overlay.raycastTarget = false;
    }

    public void LoadScene(string sceneName)
    {
        if (!transitioning)
            StartCoroutine(TransitionRoutine(sceneName));
    }

    private IEnumerator TransitionRoutine(string sceneName)
    {
        transitioning = true;
        yield return StartCoroutine(FadeRoutine(0f, 1f));
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
        yield return StartCoroutine(FadeRoutine(1f, 0f));
        transitioning = false;
    }

    private IEnumerator FadeRoutine(float from, float to)
    {
        float elapsed = 0f;
        overlay.color = new Color(0f, 0f, 0f, from);
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            overlay.color = new Color(0f, 0f, 0f, Mathf.Lerp(from, to, Mathf.Clamp01(elapsed / fadeDuration)));
            yield return null;
        }
        overlay.color = new Color(0f, 0f, 0f, to);
    }
}
