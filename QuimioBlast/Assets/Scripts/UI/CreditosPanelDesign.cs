using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Redesenha o CreditosPanel com visual de terminal robótico.
/// Adicione este script ao GameObject "CreditosPanel" dentro do CanvasMenuPrincipal.
/// O script reconstrói toda a UI automaticamente no Awake — não precisa configurar nada.
/// </summary>
[RequireComponent(typeof(Image))]
public class CreditosPanelDesign : MonoBehaviour
{
    // ── Paleta de cores ────────────────────────────────────────────────────────
    static readonly Color C_BG     = new Color(0.03f, 0.05f, 0.09f, 0.98f); // fundo escuro
    static readonly Color C_CARD   = new Color(0.06f, 0.10f, 0.16f, 1.00f); // painel central
    static readonly Color C_CYAN   = new Color(0.00f, 0.85f, 0.92f, 1.00f); // acento principal
    static readonly Color C_DIM    = new Color(0.00f, 0.38f, 0.43f, 1.00f); // ciano apagado
    static readonly Color C_TEXT   = new Color(0.88f, 0.94f, 1.00f, 1.00f); // texto principal
    static readonly Color C_PURPLE = new Color(0.54f, 0.29f, 1.00f, 1.00f); // acento botão

    static readonly string[] MEMBROS =
    {
        "Andrey Deyvison",
        "Caio César",
        "Edvaldo Felisberto",
        "Frederico Santos",
        "João Marcelo",
        "Matteus Guylherme"
    };

    private void Awake()
    {
        // Desativa e remove filhos antigos antes do primeiro frame
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
            Destroy(child.gameObject);
        }

        var bg = GetComponent<Image>();
        bg.sprite = null;
        bg.color  = C_BG;

        BuildCard();
    }

    // ── Construção do card ─────────────────────────────────────────────────────

    private void BuildCard()
    {
        // Cartão central
        var cardImg = NewImage("Card", transform, C_CARD);
        var card    = (RectTransform)cardImg.transform;
        card.anchorMin = card.anchorMax = card.pivot = new Vector2(0.5f, 0.5f);
        card.sizeDelta        = new Vector2(660, 490);
        card.anchoredPosition = Vector2.zero;
        Outline(cardImg.gameObject, C_CYAN, 2f);

        // Marcadores de canto (decoração tipo HUD)
        MakeCorner(card, ax: 0, ay: 1, px: 0, py: 1, dx:  8, dy: -8);  // ↖ TL
        MakeCorner(card, ax: 1, ay: 1, px: 1, py: 1, dx: -8, dy: -8);  // ↗ TR
        MakeCorner(card, ax: 0, ay: 0, px: 0, py: 0, dx:  8, dy:  8);  // ↙ BL
        MakeCorner(card, ax: 1, ay: 0, px: 1, py: 0, dx: -8, dy:  8);  // ↘ BR

        // ── Título ────────────────────────────────────────────────────────────
        var titleTMP = NewTMP("Title", card, "//  CRÉDITOS  //", 30, C_CYAN, FontStyles.Bold, TextAlignmentOptions.Center);
        titleTMP.characterSpacing = 5;
        SetRect(titleTMP.rectTransform, anchorMin: new Vector2(0,1), anchorMax: new Vector2(1,1),
                pivot: new Vector2(0.5f,1), pos: new Vector2(0,-20), size: new Vector2(0,52));

        var subTMP = NewTMP("Sub", card, "EQUIPE DE DESENVOLVIMENTO", 11, C_DIM, FontStyles.Normal, TextAlignmentOptions.Center);
        subTMP.characterSpacing = 4;
        SetRect(subTMP.rectTransform, new Vector2(0,1), new Vector2(1,1),
                new Vector2(0.5f,1), new Vector2(0,-76), new Vector2(0,20));

        // Separador superior
        MakeSeparator(card, anchorY: 1f, posY: -104f);

        // ── Lista de membros ──────────────────────────────────────────────────
        const float lineH = 40f;
        float       yBase = -112f;

        for (int i = 0; i < MEMBROS.Length; i++)
        {
            bool  alt   = i % 2 != 0;
            Color col   = alt ? C_DIM : C_TEXT;
            string line = (alt ? "       " : "▸  ") + MEMBROS[i];

            var mTMP = NewTMP("M" + i, card, line, 21, col, FontStyles.Normal, TextAlignmentOptions.Left);
            SetRect(mTMP.rectTransform, new Vector2(0,1), new Vector2(1,1),
                    new Vector2(0.5f,1), new Vector2(0, yBase - i * lineH), new Vector2(-60, lineH));
        }

        // Separador inferior
        MakeSeparator(card, anchorY: 0f, posY: 74f);

        // ── Rodapé ───────────────────────────────────────────────────────────
        var footTMP = NewTMP("Footer", card, "QuimioBlast  ©  2025", 11, C_DIM, FontStyles.Normal, TextAlignmentOptions.Center);
        footTMP.characterSpacing = 3;
        SetRect(footTMP.rectTransform, new Vector2(0,0), new Vector2(1,0),
                new Vector2(0.5f,0), new Vector2(0,44), new Vector2(0,22));

        // ── Botão Voltar ──────────────────────────────────────────────────────
        var btnImg = NewImage("BtnVoltar", card, C_CARD);
        SetRect((RectTransform)btnImg.transform, new Vector2(0.5f,0), new Vector2(0.5f,0),
                new Vector2(0.5f,0), new Vector2(0,12), new Vector2(220,44));
        Outline(btnImg.gameObject, C_PURPLE, 1.5f);

        var btn    = btnImg.gameObject.AddComponent<Button>();
        var colors = btn.colors;
        colors.normalColor      = C_CARD;
        colors.highlightedColor = C_PURPLE;
        colors.pressedColor     = new Color(0.3f, 0.1f, 0.6f, 1f);
        btn.colors        = colors;
        btn.targetGraphic = btnImg;
        btn.onClick.AddListener(OnVoltarClick);

        var lblTMP = NewTMP("BtnLabel", btnImg.transform, "[ VOLTAR ]", 19, C_CYAN, FontStyles.Bold, TextAlignmentOptions.Center);
        lblTMP.characterSpacing = 3;
        SetRect(lblTMP.rectTransform, Vector2.zero, Vector2.one, new Vector2(0.5f,0.5f), Vector2.zero, Vector2.zero);
    }

    private void OnVoltarClick()
    {
        FindObjectOfType<MainMenuManager>()?.FecharCreditos();
    }

    // ── Helpers ────────────────────────────────────────────────────────────────

    private Image NewImage(string name, Transform parent, Color color)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        go.AddComponent<RectTransform>();
        var img = go.AddComponent<Image>();
        img.color = color;
        return img;
    }

    private TextMeshProUGUI NewTMP(string name, Transform parent, string text, float size,
                                   Color color, FontStyles style, TextAlignmentOptions align)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        go.AddComponent<RectTransform>();
        var tmp       = go.AddComponent<TextMeshProUGUI>();
        tmp.text      = text;
        tmp.fontSize  = size;
        tmp.color     = color;
        tmp.fontStyle = style;
        tmp.alignment = align;
        return tmp;
    }

    private void SetRect(RectTransform rt, Vector2 anchorMin, Vector2 anchorMax,
                         Vector2 pivot, Vector2 pos, Vector2 size)
    {
        rt.anchorMin        = anchorMin;
        rt.anchorMax        = anchorMax;
        rt.pivot            = pivot;
        rt.anchoredPosition = pos;
        rt.sizeDelta        = size;
    }

    private void Outline(GameObject go, Color color, float width)
    {
        var o = go.AddComponent<Outline>();
        o.effectColor    = color;
        o.effectDistance = new Vector2(width, -width);
    }

    private void MakeSeparator(Transform parent, float anchorY, float posY)
    {
        var img = NewImage("Sep", parent, C_DIM);
        SetRect((RectTransform)img.transform,
                anchorMin: new Vector2(0.05f, anchorY),
                anchorMax: new Vector2(0.95f, anchorY),
                pivot:     new Vector2(0.5f, anchorY),
                pos:       new Vector2(0, posY),
                size:      new Vector2(0, 1));
    }

    private void MakeCorner(Transform parent, float ax, float ay, float px, float py, float dx, float dy)
    {
        var anchor = new Vector2(ax, ay);
        var pivot  = new Vector2(px, py);
        var pos    = new Vector2(dx, dy);

        // Linha horizontal
        var h = NewImage("CorH", parent, C_CYAN);
        SetRect((RectTransform)h.transform, anchor, anchor, pivot, pos, new Vector2(26, 2));

        // Linha vertical
        var v = NewImage("CorV", parent, C_CYAN);
        SetRect((RectTransform)v.transform, anchor, anchor, pivot, pos, new Vector2(2, 26));
    }
}
