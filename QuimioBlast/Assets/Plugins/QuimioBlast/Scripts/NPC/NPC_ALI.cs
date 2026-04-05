using UnityEngine;

public class NPC_ALI : MonoBehaviour
{
    public DialogueData dialogoInicial;
    public DialogueData[] dialogosSala1; // Arraste os 2 diálogos aqui
    public DialogueData[] dialogosSala2; // Arraste os 2 diálogos aqui

    public int salaAtual = 1;
    private bool jaTeveInicial = false;
    private int indiceS1 = 0, indiceS2 = 0;
    private bool playerInRange;

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E) && !DialogueManager.Instance.dialoguePanel.activeSelf)
            TriggerDialogue();
    }

    void TriggerDialogue()
    {
        // 1. Primeira interação do jogo
        if (!jaTeveInicial) { DialogueManager.Instance.StartDialogue(dialogoInicial); jaTeveInicial = true; return; }

        // 2. Lógica de Sequência e Repetição por Sala (RF 17)
        if (salaAtual == 1)
        {
            DialogueManager.Instance.StartDialogue(dialogosSala1[indiceS1]);
            if (indiceS1 < dialogosSala1.Length - 1) indiceS1++; // Avança até o último e trava nele
        }
        else if (salaAtual == 2)
        {
            DialogueManager.Instance.StartDialogue(dialogosSala2[indiceS2]);
            if (indiceS2 < dialogosSala2.Length - 1) indiceS2++;
        }
    }

    private void OnTriggerEnter2D(Collider2D col) { if (col.CompareTag("Player")) playerInRange = true; }
    private void OnTriggerExit2D(Collider2D col) { if (col.CompareTag("Player")) { playerInRange = false; DialogueManager.Instance.EndDialogue(); } }
}
