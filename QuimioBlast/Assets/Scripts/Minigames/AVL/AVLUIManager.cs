using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// Constrói e gerencia toda a UI do minigame BST automaticamente.
/// Não é necessário configurar nada manualmente na UI.
/// Adicione este script ao mesmo GameObject que tem AVLMinigameManager.
///
/// COMO JOGAR (exibido na UI):
///   • Clique num frasco para selecioná-lo (fica dourado)
///   • Clique num nó da árvore para colocá-lo lá
///   • Clique num nó ocupado para trocar ou pegar de volta
///   • Regra: filho ESQUERDO < pai < filho DIREITO
///   • Clique CONFIRMAR quando terminar
/// </summary>
public class AVLUIManager : MonoBehaviour
{
    // ── Tema Robô / Alquimia ───────────────────────────────────────────────
    static readonly Color C_OVERLAY  = new Color(0.02f, 0.03f, 0.06f, 0.93f);
    static readonly Color C_WINDOW   = new Color(0.05f, 0.09f, 0.15f, 1.00f);
    static readonly Color C_HEADER   = new Color(0.03f, 0.06f, 0.10f, 1.00f);
    static readonly Color C_AMBER    = new Color(1.00f, 0.57f, 0.04f, 1.00f);
    static readonly Color C_AMBERDIM = new Color(1.00f, 0.57f, 0.04f, 0.20f);
    static readonly Color C_NODEBG   = new Color(0.07f, 0.12f, 0.20f, 1.00f);
    static readonly Color C_EMPTY    = new Color(0.12f, 0.18f, 0.28f, 1.00f);
    static readonly Color C_TEXT     = new Color(0.88f, 0.83f, 0.68f, 1.00f);
    static readonly Color C_OK       = new Color(0.15f, 0.92f, 0.50f, 1.00f);
    static readonly Color C_BAD      = new Color(0.95f, 0.22f, 0.22f, 1.00f);
    static readonly Color C_SELECTED = new Color(1.00f, 0.95f, 0.10f, 1.00f);
    static readonly Color C_CONN     = new Color(1.00f, 0.57f, 0.04f, 0.28f);
    static readonly Color C_PLACED   = new Color(0.40f, 0.30f, 0.10f, 1.00f);

    // Posições dos 7 slots no painel da árvore (local ao centro)
    static readonly Vector2[] NODE_POS = {
        new Vector2(   0,  95),  // 0 raiz
        new Vector2(-155,   0),  // 1 filho esq
        new Vector2( 155,   0),  // 2 filho dir
        new Vector2(-232, -95),  // 3 neto esq-esq
        new Vector2( -78, -95),  // 4 neto esq-dir
        new Vector2(  78, -95),  // 5 neto dir-esq
        new Vector2( 232, -95),  // 6 neto dir-dir
    };
    static readonly int[,] CONN = { {0,1},{0,2},{1,3},{1,4},{2,5},{2,6} };

    // ── Refs runtime ──────────────────────────────────────────────────────
    AVLMinigameManager  mgr;
    GameObject          rootPanel;
    TextMeshProUGUI     timerTxt;
    TextMeshProUGUI     feedTxt;
    Coroutine           feedRoutine;

    // Slots
    Image[]             slotBorder = new Image[7];
    Image[]             slotInner  = new Image[7];
    TextMeshProUGUI[]   slotValTxt = new TextMeshProUGUI[7];

    // Frascos
    Image[]             flaskBorder = new Image[7];
    TextMeshProUGUI[]   flaskTxt    = new TextMeshProUGUI[7];

    // Estado de interação
    int   selFlask    = -1;       // qual frasco está selecionado (-1 = nenhum)
    int[] flaskInSlot = new int[7];  // flaskInSlot[slot] = índice do frasco naquele slot (-1 = vazio)
    int[] slotOfFlask = new int[7];  // slotOfFlask[frasco] = slot onde está (-1 = livre)

    bool validationActive = false;

    void Awake()
    {
        mgr = GetComponent<AVLMinigameManager>();
        ResetState();
        BuildUI();
    }

    // ── API pública ────────────────────────────────────────────────────────

    public void Show(AVLMinigameManager m)
    {
        mgr = m;
        ResetState();
        RefreshFlasks();
        RefreshSlots();
        rootPanel.SetActive(true);
        UpdateTimer(mgr.TimeLeft);
        if (feedTxt) feedTxt.gameObject.SetActive(false);
    }

    public void Hide()
    {
        if (rootPanel) rootPanel.SetActive(false);
    }

    public void Refresh()
    {
        RefreshFlasks();
        RefreshSlots();
    }

    public void UpdateTimer(float s)
    {
        if (!timerTxt) return;
        int v = Mathf.CeilToInt(s);
        timerTxt.text  = $"TEMPO   {v:D2}s";
        timerTxt.color = v <= 15 ? C_BAD : C_AMBER;
    }

    public void ShowFeedback(bool ok, string msg)
    {
        if (!feedTxt) return;
        if (feedRoutine != null) StopCoroutine(feedRoutine);
        feedRoutine = StartCoroutine(FeedRoutine(ok, msg));
    }

    // Colore cada slot verde (válido) ou vermelho (inválido)
    public void ShowValidation(bool[] valid)
    {
        validationActive = true;
        int okCount = 0;
        for (int i = 0; i < 7; i++)
        {
            Color border = valid[i] ? C_OK : C_BAD;
            if (slotBorder[i]) slotBorder[i].color = border;
            if (slotValTxt[i]) slotValTxt[i].color  = valid[i] ? C_OK : C_BAD;
            if (valid[i]) okCount++;
        }

        bool allOk = okCount == 7;
        string msg = allOk
            ? "✔  Árvore válida!  Parabéns!"
            : $"⚠  {okCount} de 7 corretos — corrija os nós em vermelho";
        ShowFeedback(allOk, msg);
    }

    // ── Interação ──────────────────────────────────────────────────────────

    void OnFlaskClick(int fi)
    {
        ClearValidation();
        if (selFlask == fi)
        {
            // Deselecionar
            selFlask = -1;
        }
        else if (slotOfFlask[fi] >= 0)
        {
            // Frasco está em um slot — pegar de volta
            int sl = slotOfFlask[fi];
            flaskInSlot[sl] = -1;
            slotOfFlask[fi] = -1;
            mgr.ClearSlot(sl);
            selFlask = fi;
        }
        else
        {
            // Frasco livre — selecionar
            selFlask = fi;
        }
        RefreshFlasks();
        RefreshSlots();
    }

    void OnSlotClick(int si)
    {
        ClearValidation();
        if (flaskInSlot[si] >= 0)
        {
            // Slot ocupado
            int occupant = flaskInSlot[si];
            if (selFlask >= 0)
            {
                // Trocar: colocar selecionado aqui, pegar o que estava
                flaskInSlot[si]       = selFlask;
                slotOfFlask[selFlask] = si;
                mgr.PlaceNumber(si, mgr.Numbers[selFlask]);

                slotOfFlask[occupant] = -1;
                selFlask = occupant;   // agora segurando o que foi retirado
            }
            else
            {
                // Pegar o que está no slot
                selFlask        = occupant;
                flaskInSlot[si] = -1;
                slotOfFlask[occupant] = -1;
                mgr.ClearSlot(si);
            }
        }
        else
        {
            // Slot vazio
            if (selFlask >= 0)
            {
                // Se o frasco selecionado já estava em outro slot, libera o antigo
                if (slotOfFlask[selFlask] >= 0)
                {
                    int oldSlot = slotOfFlask[selFlask];
                    flaskInSlot[oldSlot] = -1;
                    mgr.ClearSlot(oldSlot);
                }
                flaskInSlot[si]       = selFlask;
                slotOfFlask[selFlask] = si;
                mgr.PlaceNumber(si, mgr.Numbers[selFlask]);
                selFlask = -1;
            }
        }
        RefreshFlasks();
        RefreshSlots();
    }

    // ── Refresh visual ─────────────────────────────────────────────────────

    void RefreshFlasks()
    {
        for (int fi = 0; fi < 7; fi++)
        {
            bool placed   = slotOfFlask[fi] >= 0;
            bool selected = selFlask == fi;

            Color border;
            Color txt;
            if (selected)       { border = C_SELECTED; txt = new Color(0.05f, 0.04f, 0.01f); }
            else if (placed)    { border = C_PLACED;   txt = new Color(C_TEXT.r, C_TEXT.g, C_TEXT.b, 0.35f); }
            else                { border = C_AMBER;    txt = C_TEXT; }

            if (flaskBorder[fi]) flaskBorder[fi].color = border;
            if (flaskTxt[fi])
            {
                flaskTxt[fi].text  = mgr != null ? mgr.Numbers[fi].ToString() : "?";
                flaskTxt[fi].color = txt;
            }
        }
    }

    void RefreshSlots()
    {
        for (int si = 0; si < 7; si++)
        {
            int  fi       = flaskInSlot[si];
            bool occupied = fi >= 0;

            Color border  = occupied ? C_AMBER   : C_AMBERDIM;
            Color inner   = occupied ? C_NODEBG  : C_EMPTY;
            string val    = occupied && mgr != null ? mgr.Numbers[fi].ToString() : "?";
            Color valCol  = occupied ? C_TEXT : new Color(C_TEXT.r, C_TEXT.g, C_TEXT.b, 0.25f);

            if (!validationActive)
            {
                if (slotBorder[si]) slotBorder[si].color = border;
                if (slotValTxt[si]) slotValTxt[si].color  = valCol;
            }
            if (slotInner[si])  slotInner[si].color  = inner;
            if (slotValTxt[si]) slotValTxt[si].text   = val;
        }
    }

    void ClearValidation()
    {
        if (!validationActive) return;
        validationActive = false;
        if (feedTxt) feedTxt.gameObject.SetActive(false);
        RefreshSlots();
    }

    void ResetState()
    {
        selFlask = -1;
        for (int i = 0; i < 7; i++) { flaskInSlot[i] = -1; slotOfFlask[i] = -1; }
        validationActive = false;
    }

    // ── Coroutine de feedback ──────────────────────────────────────────────

    IEnumerator FeedRoutine(bool ok, string msg)
    {
        feedTxt.gameObject.SetActive(true);
        feedTxt.text  = msg;
        feedTxt.color = ok ? C_OK : C_BAD;
        if (!ok)
        {
            yield return new WaitForSeconds(3f);
            feedTxt.gameObject.SetActive(false);
        }
        feedRoutine = null;
    }

    // ── Construção da UI ───────────────────────────────────────────────────

    void BuildUI()
    {
        Canvas cv = FindObjectOfType<Canvas>();
        if (!cv)
        {
            var cgo = MakeGO("Canvas_AVL", null);
            cv = cgo.AddComponent<Canvas>();
            cv.renderMode = RenderMode.ScreenSpaceOverlay;
            cgo.AddComponent<CanvasScaler>();
            cgo.AddComponent<GraphicRaycaster>();
        }

        // Overlay escuro fullscreen
        rootPanel = MakeGO("AVL_Overlay", cv.transform);
        Fill(rootPanel);
        AddImage(rootPanel, C_OVERLAY);
        rootPanel.SetActive(false);

        // Janela central
        var win = MakeGO("AVL_Window", rootPanel.transform);
        SetAnch(win, .5f,.5f,.5f,.5f, 760, 610, 0, 0);
        AddImage(win, C_WINDOW);
        AddOutline(win, C_AMBER, 2);
        AddVLG(win, pad:10, spacing:6);

        MakeHeader(win.transform);
        MakeRuleRow(win.transform);
        MakeTreeArea(win.transform);
        MakeFlaskLabel(win.transform);
        MakeFlaskRow(win.transform);
        MakeFeedRow(win.transform);
        MakeCtrlRow(win.transform);
    }

    void MakeHeader(Transform p)
    {
        var h = MakeGO("Header", p);
        AddLE(h, ph:44); AddImage(h, C_HEADER); AddOutline(h, C_AMBER, 1);
        AddHLG(h, pad:12, spacing:0);

        var title = MakeTMP("Title", h.transform, "⚗   ORDENAR A ÁRVORE BINÁRIA   ⚗",
            C_AMBER, 16, TextAlignmentOptions.MidlineLeft, bold: true);
        AddLE(title.gameObject, pw:530); StretchFill(title.gameObject);

        timerTxt = MakeTMP("Timer", h.transform, "TEMPO  90s",
            C_AMBER, 15, TextAlignmentOptions.MidlineRight);
        AddLE(timerTxt.gameObject, pw:190); StretchFill(timerTxt.gameObject);
    }

    void MakeRuleRow(Transform p)
    {
        var bg = MakeGO("RuleRow", p);
        AddLE(bg, ph:30); AddImage(bg, new Color(0.04f, 0.08f, 0.13f, 1f));
        AddHLG(bg, pad:16, spacing:0);

        MakeTMP("Rule", bg.transform,
            "ESQUERDA  ←  menor que o pai      |      maior que o pai  →  DIREITA",
            new Color(C_AMBER.r, C_AMBER.g, C_AMBER.b, 0.75f), 12, TextAlignmentOptions.Center);
    }

    void MakeTreeArea(Transform p)
    {
        var area = MakeGO("TreeArea", p);
        AddLE(area, ph:272);
        AddImage(area, new Color(0.03f, 0.06f, 0.11f, 1f));
        AddOutline(area, new Color(C_AMBER.r, C_AMBER.g, C_AMBER.b, 0.30f), 1);

        // Linhas conectoras (desenhadas antes dos nós para ficarem atrás)
        for (int i = 0; i < CONN.GetLength(0); i++)
            MakeConnector(area.transform, NODE_POS[CONN[i,0]], NODE_POS[CONN[i,1]]);

        for (int i = 0; i < 7; i++)
            MakeSlot(area.transform, i);
    }

    void MakeSlot(Transform p, int si)
    {
        var outer = MakeGO($"Slot_{si}", p);
        SetAnch(outer, .5f,.5f,.5f,.5f, 78, 68, NODE_POS[si].x, NODE_POS[si].y);
        slotBorder[si] = AddImage(outer, C_AMBERDIM);

        var inner = MakeGO("In", outer.transform);
        Fill(inner, margin:2);
        slotInner[si] = AddImage(inner, C_EMPTY);

        slotValTxt[si] = MakeTMP("V", inner.transform, "?",
            new Color(C_TEXT.r, C_TEXT.g, C_TEXT.b, 0.25f), 23,
            TextAlignmentOptions.Center, bold: true);
        StretchFill(slotValTxt[si].gameObject);

        int capture = si;
        var btn = outer.AddComponent<Button>();
        var cb  = ColorBlock.defaultColorBlock;
        cb.normalColor      = Color.white;
        cb.highlightedColor = new Color(1f, 0.85f, 0.35f, 1f);
        cb.pressedColor     = new Color(0.8f, 0.7f, 0.2f, 1f);
        cb.fadeDuration     = 0.08f;
        btn.colors = cb;
        btn.onClick.AddListener(() => OnSlotClick(capture));
    }

    void MakeFlaskLabel(Transform p)
    {
        var row = MakeGO("FlaskLbl", p);
        AddLE(row, ph:22); AddImage(row, new Color(0,0,0,0));
        MakeTMP("L", row.transform,
            "— FRASCOS —   Clique para selecionar, depois clique em um nó da árvore",
            new Color(C_AMBER.r, C_AMBER.g, C_AMBER.b, 0.65f), 11, TextAlignmentOptions.Center);
    }

    void MakeFlaskRow(Transform p)
    {
        var row = MakeGO("FlaskRow", p);
        AddLE(row, ph:74); AddImage(row, C_HEADER); AddOutline(row, C_AMBER, 1);
        var hlg = row.AddComponent<HorizontalLayoutGroup>();
        hlg.padding = new RectOffset(8,8,8,8); hlg.spacing = 5;
        hlg.childAlignment       = TextAnchor.MiddleCenter;
        hlg.childControlWidth    = hlg.childControlHeight    = true;
        hlg.childForceExpandWidth = hlg.childForceExpandHeight = false;

        for (int i = 0; i < 7; i++)
        {
            int fi = i;
            var flask = MakeGO($"Flask_{i}", row.transform);
            AddLE(flask, pw:96, ph:58);
            flaskBorder[fi] = AddImage(flask, C_AMBER);

            var inner = MakeGO("In", flask.transform);
            Fill(inner, margin:2);
            AddImage(inner, C_NODEBG);

            flaskTxt[fi] = MakeTMP("V", inner.transform, "?",
                C_TEXT, 22, TextAlignmentOptions.Center, bold: true);
            StretchFill(flaskTxt[fi].gameObject);

            var btn = flask.AddComponent<Button>();
            var cb  = ColorBlock.defaultColorBlock;
            cb.normalColor      = Color.white;
            cb.highlightedColor = new Color(1f, 0.9f, 0.3f, 1f);
            cb.pressedColor     = new Color(0.8f, 0.75f, 0.2f, 1f);
            cb.fadeDuration     = 0.08f;
            btn.colors = cb;
            btn.onClick.AddListener(() => OnFlaskClick(fi));
        }
    }

    void MakeFeedRow(Transform p)
    {
        var row = MakeGO("FeedRow", p);
        AddLE(row, ph:30); AddImage(row, new Color(0,0,0,0));
        feedTxt = MakeTMP("Feed", row.transform, "",
            C_OK, 13, TextAlignmentOptions.Center, bold: true);
        feedTxt.gameObject.SetActive(false);
    }

    void MakeCtrlRow(Transform p)
    {
        var row = MakeGO("CtrlRow", p);
        AddLE(row, ph:54); AddImage(row, C_HEADER); AddOutline(row, C_AMBER, 1);
        AddHLG(row, pad:12, spacing:10);

        // Spacer esquerdo
        var sp = MakeGO("Sp", row.transform); AddLE(sp, fw:1); AddImage(sp, new Color(0,0,0,0));

        // Botão limpar
        var clrGO = MakeButton(row.transform, "LIMPAR TUDO", new Color(0.2f,0.10f,0.03f,1f), C_AMBER, 12);
        AddLE(clrGO, pw:140); AddOutline(clrGO, C_AMBER, 1);
        clrGO.GetComponent<Button>().onClick.AddListener(() =>
        {
            mgr?.ClearAll();
            ResetState();
            RefreshFlasks();
            RefreshSlots();
        });

        // Botão confirmar
        var confGO = MakeButton(row.transform, "✔  CONFIRMAR", new Color(0.08f,0.26f,0.10f,1f), C_OK, 13, bold:true);
        AddLE(confGO, pw:155); AddOutline(confGO, C_OK, 1);
        confGO.GetComponent<Button>().onClick.AddListener(() => mgr?.ConfirmPlacement());

        // Botão fechar
        var closeGO = MakeButton(row.transform, "✖  FECHAR", new Color(0.26f,0.07f,0.07f,1f), C_BAD, 12);
        AddLE(closeGO, pw:120); AddOutline(closeGO, C_BAD, 1);
        closeGO.GetComponent<Button>().onClick.AddListener(() => mgr?.CloseMinigame());
    }

    // ── Helpers de criação ─────────────────────────────────────────────────

    GameObject MakeGO(string n, Transform p)
    {
        var g = new GameObject(n);
        if (p != null) g.transform.SetParent(p, false);
        g.AddComponent<RectTransform>();
        return g;
    }

    void Fill(GameObject g, int margin = 0)
    {
        var r = g.GetComponent<RectTransform>();
        r.anchorMin = Vector2.zero; r.anchorMax = Vector2.one;
        r.offsetMin = new Vector2(margin, margin);
        r.offsetMax = new Vector2(-margin, -margin);
    }

    void StretchFill(GameObject g)
    {
        var r = g.GetComponent<RectTransform>();
        r.anchorMin = Vector2.zero; r.anchorMax = Vector2.one;
        r.sizeDelta = Vector2.zero; r.anchoredPosition = Vector2.zero;
    }

    void SetAnch(GameObject g, float axn, float ayn, float axx, float axy, float w, float h, float px, float py)
    {
        var r = g.GetComponent<RectTransform>();
        r.anchorMin = new Vector2(axn, ayn); r.anchorMax = new Vector2(axx, axy);
        r.sizeDelta = new Vector2(w, h);     r.anchoredPosition = new Vector2(px, py);
    }

    Image AddImage(GameObject g, Color c)
    {
        var img = g.AddComponent<Image>(); img.color = c; return img;
    }

    void AddOutline(GameObject g, Color c, int t)
    {
        var o = g.AddComponent<Outline>();
        o.effectColor = c; o.effectDistance = new Vector2(t, -t);
    }

    void AddLE(GameObject g, float ph = -1, float pw = -1, float fw = -1)
    {
        var le = g.AddComponent<LayoutElement>();
        if (ph >= 0) le.preferredHeight = ph;
        if (pw >= 0) le.preferredWidth  = pw;
        if (fw >= 0) le.flexibleWidth   = fw;
    }

    void AddVLG(GameObject g, int pad = 0, int spacing = 0)
    {
        var v = g.AddComponent<VerticalLayoutGroup>();
        v.padding = new RectOffset(pad,pad,pad,pad); v.spacing = spacing;
        v.childAlignment       = TextAnchor.UpperCenter;
        v.childControlWidth    = v.childControlHeight    = true;
        v.childForceExpandWidth  = true;
        v.childForceExpandHeight = false;
    }

    void AddHLG(GameObject g, int pad = 0, int spacing = 0)
    {
        var h = g.AddComponent<HorizontalLayoutGroup>();
        h.padding = new RectOffset(pad,pad,pad,pad); h.spacing = spacing;
        h.childAlignment       = TextAnchor.MiddleLeft;
        h.childControlWidth    = h.childControlHeight    = true;
        h.childForceExpandWidth = h.childForceExpandHeight = false;
    }

    TextMeshProUGUI MakeTMP(string n, Transform p, string txt, Color c, float sz,
        TextAlignmentOptions al, bool bold = false)
    {
        var g = MakeGO(n, p);
        var t = g.AddComponent<TextMeshProUGUI>();
        t.text = txt; t.color = c; t.fontSize = sz; t.alignment = al;
        if (bold) t.fontStyle = FontStyles.Bold;
        StretchFill(g);
        return t;
    }

    GameObject MakeButton(Transform p, string label, Color bg, Color fg, float sz, bool bold = false)
    {
        var g   = MakeGO("Btn", p);
        AddImage(g, bg);
        g.AddComponent<Button>();
        var lbl = MakeGO("L", g.transform);
        Fill(lbl, margin:4);
        var t = lbl.AddComponent<TextMeshProUGUI>();
        t.text = label; t.color = fg; t.fontSize = sz;
        t.alignment = TextAlignmentOptions.Center;
        if (bold) t.fontStyle = FontStyles.Bold;
        return g;
    }

    void MakeConnector(Transform p, Vector2 a, Vector2 b)
    {
        var g = MakeGO("Ln", p);
        SetAnch(g, .5f,.5f,.5f,.5f, 0,0,0,0);
        AddImage(g, C_CONN);
        var r = g.GetComponent<RectTransform>();
        var d = b - a;
        r.sizeDelta        = new Vector2(d.magnitude, 2f);
        r.anchoredPosition = (a + b) * 0.5f;
        r.localRotation    = Quaternion.Euler(0, 0, Mathf.Atan2(d.y, d.x) * Mathf.Rad2Deg);
    }
}
