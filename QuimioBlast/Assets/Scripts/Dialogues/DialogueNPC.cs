using UnityEngine;
using TMPro;

public class DialogueNPC : MonoBehaviour
{
    public const string TALKED_KEY_PREFIX = "NPC_Talked_";

    [Header("Início da Conversa")]
    [Tooltip("Arraste o primeiro Nó de diálogo deste NPC aqui.")]
    public DialogueNode noInicial;

    [Header("Rastreamento")]
    [Tooltip("ID único deste NPC. Deve coincidir com o configurado no CenaInicialGatekeeper (ex: ALI, Leo, Reko).")]
    [SerializeField] private string npcId = "";

    [Header("Visual Feedback")]
    [Tooltip("Opcional: arrastar um GameObject indicador. Se vazio, um '[F]' amarelo é criado automaticamente.")]
    public GameObject interactionPrompt;

    private bool playerInRange = false;

    private void Start()
    {
        if (interactionPrompt == null)
            interactionPrompt = CriarPromptF();

        interactionPrompt.SetActive(false);
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.F))
        {
            if (!DialogueManager.Instance.dialoguePanel.activeSelf && noInicial != null)
            {
                if (!string.IsNullOrEmpty(npcId))
                    PlayerPrefs.SetInt(TALKED_KEY_PREFIX + npcId, 1);
                DialogueManager.Instance.StartDialogue(noInicial);
            }
        }
    }

    private GameObject CriarPromptF()
    {
        var obj = new GameObject("InteractionPrompt");
        obj.transform.SetParent(transform);
        obj.transform.localPosition = new Vector3(0f, 3f, 0f);
        obj.transform.localScale = Vector3.one * 0.5f;

        var tmp = obj.AddComponent<TextMeshPro>();
        tmp.text = "[F]";
        tmp.fontSize = 8;
        tmp.color = Color.yellow;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontStyle = FontStyles.Bold;

        // Garante que o texto fica visível na frente do sprite do NPC
        if (obj.TryGetComponent<MeshRenderer>(out var mr))
            mr.sortingOrder = 10;

        return obj;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
            if (interactionPrompt != null) interactionPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
            if (interactionPrompt != null) interactionPrompt.SetActive(false);
            DialogueManager.Instance.EndDialogue();
        }
    }
}
