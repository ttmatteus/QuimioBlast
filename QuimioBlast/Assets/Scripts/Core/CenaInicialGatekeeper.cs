using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Adicione este script a um GameObject com Collider2D (isTrigger = true)
/// na saída da CenaInicial.
/// Quando o player tocar o trigger:
///   - Todos os NPCs falados → carrega a próxima cena
///   - NPCs pendentes       → exibe mensagem pedindo que fale com todos primeiro
/// Inspector: preencha requiredNpcIds com os mesmos IDs configurados em cada DialogueNPC.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class CenaInicialGatekeeper : MonoBehaviour
{
    [SerializeField] private string playerTag    = "Player";
    [SerializeField] private string nextScene    = "PrimeiraFaseNormal";
    [SerializeField] private float  msgDuration  = 3.5f;

    [Tooltip("IDs dos NPCs obrigatórios — devem coincidir com o campo npcId em cada DialogueNPC.")]
    [SerializeField] private string[] requiredNpcIds = { "ALI", "Leo", "Reko" };

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

        if (AllNpcsVisited())
        {
            SceneTransition.Instance.LoadScene(nextScene);
        }
        else if (!showingMessage)
        {
            StartCoroutine(ShowInfoMessage("Fale com todos os NPCs antes de avançar!"));
        }
    }

    private bool AllNpcsVisited()
    {
        foreach (string id in requiredNpcIds)
        {
            if (PlayerPrefs.GetInt(DialogueNPC.TALKED_KEY_PREFIX + id, 0) == 0)
                return false;
        }
        return true;
    }

    private IEnumerator ShowInfoMessage(string message)
    {
        showingMessage = true;
        GameObject canvasGO = BuildMessageUI(message);
        yield return new WaitForSecondsRealtime(msgDuration);
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
        rt.sizeDelta        = new Vector2(750, 80);
        rt.anchoredPosition = new Vector2(0, -50);

        var bg   = panel.AddComponent<Image>();
        bg.color = C_BG;

        var outline            = panel.AddComponent<Outline>();
        outline.effectColor    = C_AMBER;
        outline.effectDistance = new Vector2(2, -2);

        var textGO = new GameObject("Text");
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
