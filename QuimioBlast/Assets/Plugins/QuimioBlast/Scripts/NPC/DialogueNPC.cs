using UnityEngine;

public class DialogueNPC : MonoBehaviour
{
    [Header("Início da Conversa")]
    [Tooltip("Arraste o primeiro Nó de diálogo deste NPC aqui.")]
    public DialogueNode noInicial;

    private bool playerInRange = false;

    void Update()
    {
        // Se o player está perto, aperta 'E' e a caixa não está aberta
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (!DialogueManager.Instance.dialoguePanel.activeSelf && noInicial != null)
            {
                DialogueManager.Instance.StartDialogue(noInicial);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
            DialogueManager.Instance.EndDialogue();
        }
    }
}