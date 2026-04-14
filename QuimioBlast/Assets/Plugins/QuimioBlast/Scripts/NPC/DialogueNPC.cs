using UnityEngine;

public class DialogueNPC : MonoBehaviour
{
    [Header("In�cio da Conversa")]
    [Tooltip("Arraste o primeiro N� de di�logo deste NPC aqui.")]
    public DialogueNode noInicial;

    private bool playerInRange = false;

    void Update()
    {
        // Se o player est� perto, aperta 'E' e a caixa n�o est� aberta
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (!DialogueManager.Instance.dialoguePanel.activeSelf && noInicial != null)
            {
                DialogueManager.Instance.StartDialogue(noInicial);
            }
        }
    }

    [Header("Visual Feedback")]
    public GameObject interactionPrompt; // Uma imagem de tecla 'E' flutuando

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) 
        {
            playerInRange = true;
            if(interactionPrompt != null) interactionPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
            if(interactionPrompt != null) interactionPrompt.SetActive(false);
            DialogueManager.Instance.EndDialogue();
        }
    }
    
    
}

