using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Adicione este script a um GameObject com Collider2D (isTrigger = true)
/// posicionado na entrada da área da boss fight na cena PrimeiraFaseNormal.
/// Quando o player tocar o trigger:
///   - AVL resolvido  → carrega PrimeiraFaseBoss
///   - AVL pendente   → exibe mensagem pedindo que resolva o desafio primeiro
/// Inspector: confirme que o Player tem a Tag "Player".
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class PrimeiraFaseGatekeeper : MonoBehaviour
{
    [SerializeField] private string playerTag       = "Player";
    [SerializeField] private string bossSceneName   = "PrimeiraFaseBoss";
    [SerializeField] private float  messageDuration = 3.5f;

    private static readonly Color C_AMBER = new Color(1.00f, 0.57f, 0.04f, 1f);
    private static readonly Color C_BG    = new Color(0.04f, 0.07f, 0.12f, 0.92f);

    private bool showingMessage;

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        if (PlayerPrefs.GetInt(AVLMinigameManager.AVL_SOLVED_KEY, 0) == 1)
        {
            SceneTransition.Instance.LoadScene(bossSceneName);
        }
        else if (!showingMessage)
        {
            StartCoroutine(ShowInfoMessage("Resolva o Desafio AVL antes de enfrentar o boss!"));
        }
    }

    private IEnumerator ShowInfoMessage(string message)
    {
        showingMessage = true;
        GameObject canvasGO = BuildMessageUI(message);
        yield return new WaitForSecondsRealtime(messageDuration);
        if (canvasGO != null) Destroy(canvasGO);
        showingMessage = false;
    }

    private GameObject BuildMessageUI(string message)
    {
        var canvasGO = new GameObject("GatekeeperCanvas");
        var canvas   = canvasGO.AddComponent<Canvas>();
        canvas.renderMode   = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 20;
        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasGO.AddComponent<GraphicRaycaster>();

        var panel = new GameObject("Panel");
        panel.transform.SetParent(canvasGO.transform, false);
        var rt              = panel.AddComponent<RectTransform>();
        rt.anchorMin        = new Vector2(0.5f, 1f);
        rt.anchorMax        = new Vector2(0.5f, 1f);
        rt.pivot            = new Vector2(0.5f, 1f);
        rt.sizeDelta        = new Vector2(700, 80);
        rt.anchoredPosition = new Vector2(0, -50);

        var bg   = panel.AddComponent<Image>();
        bg.color = C_BG;

        var outline            = panel.AddComponent<Outline>();
        outline.effectColor    = C_AMBER;
        outline.effectDistance = new Vector2(2, -2);

        var textGO  = new GameObject("Text");
        textGO.transform.SetParent(panel.transform, false);
        var textRT              = textGO.AddComponent<RectTransform>();
        textRT.anchorMin        = Vector2.zero;
        textRT.anchorMax        = Vector2.one;
        textRT.sizeDelta        = Vector2.zero;
        textRT.anchoredPosition = Vector2.zero;

        var tmp       = textGO.AddComponent<TextMeshProUGUI>();
        tmp.text      = message;
        tmp.color     = C_AMBER;
        tmp.fontSize  = 22;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Center;

        return canvasGO;
    }
}
