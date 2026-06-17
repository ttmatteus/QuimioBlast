using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// Zona de interação que cria seu próprio prompt visual automaticamente.
/// Não é necessário criar nada na UI manualmente.
/// Inspector: arraste AVLMinigameManager no campo Minigame Manager.
/// Confirme que o Player tem a Tag "Player".
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class AVLInteractionZone : MonoBehaviour
{
    [SerializeField] private AVLMinigameManager minigameManager;
    [SerializeField] private string playerTag = "Player";

    private static readonly Color C_AMBER = new Color(1.00f, 0.57f, 0.04f, 1f);
    private static readonly Color C_BG    = new Color(0.04f, 0.07f, 0.12f, 0.92f);

    private bool              playerInside;
    private GameObject        promptRoot;
    private TextMeshProUGUI   promptText;
    private Image             promptBG;
    private Coroutine         pulseRoutine;

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
        BuildPromptUI();
    }

    private void Update()
    {
        if (!playerInside || minigameManager == null) return;
        if (minigameManager.IsLocked || minigameManager.IsRunning) return;
        if (Input.GetKeyDown(KeyCode.F))
            minigameManager.OpenMinigame();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        playerInside = true;
        ShowPrompt(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        playerInside = false;
        ShowPrompt(false);
    }

    // ── Prompt visual ──────────────────────────────────────────────────────

    private void BuildPromptUI()
    {
        // Canvas dedicado para o prompt (não interfere com outros Canvas)
        var canvasGO = new GameObject("AVL_PromptCanvas");
        var canvas   = canvasGO.AddComponent<Canvas>();
        canvas.renderMode   = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 8;
        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasGO.AddComponent<GraphicRaycaster>();

        // Painel do prompt — parte inferior da tela, centralizado
        promptRoot = new GameObject("Prompt");
        promptRoot.transform.SetParent(canvasGO.transform, false);
        var rt = promptRoot.AddComponent<RectTransform>();
        rt.anchorMin        = new Vector2(0.5f, 0f);
        rt.anchorMax        = new Vector2(0.5f, 0f);
        rt.pivot            = new Vector2(0.5f, 0f);
        rt.sizeDelta        = new Vector2(400, 56);
        rt.anchoredPosition = new Vector2(0, 70);

        promptBG = promptRoot.AddComponent<Image>();
        promptBG.color = C_BG;

        var outline = promptRoot.AddComponent<Outline>();
        outline.effectColor    = C_AMBER;
        outline.effectDistance = new Vector2(2, -2);

        // Texto do prompt
        var textGO = new GameObject("Text");
        textGO.transform.SetParent(promptRoot.transform, false);
        var textRT = textGO.AddComponent<RectTransform>();
        textRT.anchorMin        = Vector2.zero;
        textRT.anchorMax        = Vector2.one;
        textRT.sizeDelta        = Vector2.zero;
        textRT.anchoredPosition = Vector2.zero;

        promptText           = textGO.AddComponent<TextMeshProUGUI>();
        promptText.text      = "[ F ]   Resolver Desafio AVL";
        promptText.color     = C_AMBER;
        promptText.fontSize  = 18;
        promptText.fontStyle = FontStyles.Bold;
        promptText.alignment = TextAlignmentOptions.Center;

        promptRoot.SetActive(false);
    }

    private void ShowPrompt(bool show)
    {
        if (promptRoot == null) return;

        bool blocked = minigameManager != null && minigameManager.IsLocked;
        promptRoot.SetActive(show && !blocked);

        if (show && !blocked)
        {
            if (pulseRoutine != null) StopCoroutine(pulseRoutine);
            pulseRoutine = StartCoroutine(PulseRoutine());
        }
        else
        {
            if (pulseRoutine != null) { StopCoroutine(pulseRoutine); pulseRoutine = null; }
        }
    }

    // Pulsa o brilho do texto e da borda para chamar atenção
    private IEnumerator PulseRoutine()
    {
        float t = 0f;
        while (true)
        {
            t += Time.deltaTime * 2.2f;
            float alpha = Mathf.Lerp(0.45f, 1f, (Mathf.Sin(t) + 1f) * 0.5f);

            if (promptText != null)
                promptText.color = new Color(C_AMBER.r, C_AMBER.g, C_AMBER.b, alpha);

            if (promptBG != null)
                promptBG.color = new Color(C_BG.r, C_BG.g, C_BG.b,
                    Mathf.Lerp(0.75f, 0.95f, (Mathf.Sin(t) + 1f) * 0.5f));

            yield return null;
        }
    }
}
