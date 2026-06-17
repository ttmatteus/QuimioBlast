using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Gerencia o Minigame "Ordenar a Árvore BST".
///
/// Regra do jogo: 7 frascos com números aparecem embaralhados.
/// O jogador os posiciona nos 7 nós de uma árvore binária.
/// Ao confirmar, o jogo verifica se a posição de cada nó respeita
/// a propriedade BST: filho esquerdo < pai < filho direito.
///
/// Setup no Inspector:
///   • Player Ref → GameObject do Player
///   • On Challenge Solved → evento para abrir porta, etc.
/// </summary>
public class AVLMinigameManager : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private GameObject playerRef;

    [Header("Tempos")]
    [SerializeField] private float timeLimit    = 90f;
    [SerializeField] private float lockDuration = 10f;

    [Header("Evento de Vitória")]
    public UnityEvent OnChallengeSolved;

    // ── Estado público ─────────────────────────────────────────────────────
    public int[]  Numbers   { get; private set; }   // os 7 valores dos frascos
    public int?[] Slots     { get; private set; }   // o que está em cada slot (null = vazio)
    public float  TimeLeft  { get; private set; }
    public bool   IsRunning { get; private set; }
    public bool   IsLocked  { get; private set; }

    private AVLUIManager     uiManager;
    private PlayerMovement2D playerMovement;
    private Coroutine        timerRoutine;

    private void Awake()
    {
        if (playerRef != null)
            playerMovement = playerRef.GetComponent<PlayerMovement2D>();

        uiManager = GetComponent<AVLUIManager>();
        Numbers   = new int[7];
        Slots     = new int?[7];
    }

    // ── API pública ────────────────────────────────────────────────────────

    public void OpenMinigame()
    {
        if (IsLocked || IsRunning) return;

        GenerateNumbers();
        System.Array.Clear(Slots, 0, Slots.Length);

        TimeLeft  = timeLimit;
        IsRunning = true;

        FreezePlayer(true);
        uiManager.Show(this);
        timerRoutine = StartCoroutine(TimerRoutine());
    }

    public void CloseMinigame()
    {
        StopTimer();
        IsRunning = false;
        FreezePlayer(false);
        uiManager.Hide();
    }

    // Chamado pela UI para registrar a colocação de um frasco num slot
    public void PlaceNumber(int slotIndex, int value)
    {
        Slots[slotIndex] = value;
    }

    public void ClearSlot(int slotIndex)
    {
        Slots[slotIndex] = null;
    }

    public void ClearAll()
    {
        System.Array.Clear(Slots, 0, Slots.Length);
        uiManager.Refresh();
    }

    // Chamado pelo botão CONFIRMAR da UI
    public void ConfirmPlacement()
    {
        if (!AllFilled())
        {
            uiManager.ShowFeedback(false, "Coloque um frasco em cada nó!");
            return;
        }

        bool[] valid = GetValidation();
        uiManager.ShowValidation(valid);

        bool allOk = true;
        foreach (bool v in valid) if (!v) { allOk = false; break; }

        if (allOk)
            StartCoroutine(VictoryRoutine());
    }

    // Retorna valid[i] = true se o nó i satisfaz a propriedade BST
    public bool[] GetValidation()
    {
        var valid = new bool[7];
        for (int i = 0; i < 7; i++) valid[i] = Slots[i] != null;
        if (Slots[0] != null)
            ValidateNode(0, int.MinValue, int.MaxValue, valid);
        return valid;
    }

    public bool AllFilled()
    {
        for (int i = 0; i < 7; i++) if (Slots[i] == null) return false;
        return true;
    }

    // ── Privado ────────────────────────────────────────────────────────────

    // Gera 7 números distintos e embaralha
    private void GenerateNumbers()
    {
        var used = new HashSet<int>();
        for (int i = 0; i < 7; i++)
        {
            int n;
            do { n = Random.Range(5, 99); } while (used.Contains(n));
            used.Add(n);
            Numbers[i] = n;
        }
        // Fisher-Yates shuffle
        for (int i = 6; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (Numbers[i], Numbers[j]) = (Numbers[j], Numbers[i]);
        }
    }

    // Validação BST recursiva com limites herdados
    // Índices: 0=raiz, filhoEsq(i)=2i+1, filhoDir(i)=2i+2
    private void ValidateNode(int i, int min, int max, bool[] valid)
    {
        if (i >= 7 || Slots[i] == null) return;
        int val = Slots[i].Value;
        if (val <= min || val >= max) { valid[i] = false; return; }
        ValidateNode(2 * i + 1, min, val, valid);
        ValidateNode(2 * i + 2, val, max, valid);
    }

    public const string AVL_SOLVED_KEY = "AVL_PrimeiraFase_Solved";

    private IEnumerator VictoryRoutine()
    {
        yield return new WaitForSeconds(1.8f);
        StopTimer();
        IsRunning = false;
        FreezePlayer(false);
        uiManager.Hide();
        PlayerPrefs.SetInt(AVL_SOLVED_KEY, 1);
        PlayerPrefs.Save();
        OnChallengeSolved?.Invoke();
    }

    private IEnumerator TimerRoutine()
    {
        while (TimeLeft > 0f)
        {
            yield return null;
            TimeLeft -= Time.deltaTime;
            uiManager.UpdateTimer(TimeLeft);
        }
        TimeLeft = 0f;
        uiManager.UpdateTimer(0f);
        HandleFailure();
    }

    private void HandleFailure()
    {
        IsRunning = false;
        FreezePlayer(false);
        uiManager.Hide();
        StartCoroutine(LockRoutine());
    }

    private IEnumerator LockRoutine()
    {
        IsLocked = true;
        yield return new WaitForSeconds(lockDuration);
        IsLocked = false;
    }

    private void StopTimer()
    {
        if (timerRoutine != null) { StopCoroutine(timerRoutine); timerRoutine = null; }
    }

    private void FreezePlayer(bool freeze)
    {
        if (playerMovement == null) return;
        if (freeze) playerMovement.PararMovimento();
        playerMovement.enabled = !freeze;
    }
}
