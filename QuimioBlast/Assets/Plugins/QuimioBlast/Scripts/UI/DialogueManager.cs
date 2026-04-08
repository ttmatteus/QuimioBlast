using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    public GameObject dialoguePanel;
    public TextMeshProUGUI speakerText;
    public TextMeshProUGUI dialogueText;

    [Header("Opções")]
    public GameObject buttonPrefab; // Prefab de um botão de UI
    public Transform buttonContainer; // Onde os botões vão ficar (ex: um Vertical Layout Group)

    private void Awake() { Instance = this; }

    public void StartDialogue(DialogueNode startNode)
    {   
        Time.timeScale = 0f; // Pausa o jogo durante o diálogo (Requisito do professor)
        dialoguePanel.SetActive(true);
        DisplayNode(startNode);
    }

    public void DisplayNode(DialogueNode node)
    {
        speakerText.text = node.speakerName;
        dialogueText.text = node.dialogueText;

        // Executa a ação do nó, se houver
        node.onNodeEnter?.Invoke();

        // Limpa botões antigos
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }

        // VERIFICAÇÃO DE SEGURANÇA: Se não houver opções, cria um botão de Sair
        if (node.choices == null || node.choices.Length == 0)
        {
            GameObject btnObj = Instantiate(buttonPrefab, buttonContainer);
            btnObj.GetComponentInChildren<TextMeshProUGUI>().text = "Sair";
            btnObj.GetComponent<Button>().onClick.AddListener(() => {
                EndDialogue(); // Chama o fim do diálogo direto
            });
            return; // Interrompe a função aqui
        }

        // Cria novos botões para cada opção normalmente
        foreach (var choice in node.choices)
        {
            GameObject btnObj = Instantiate(buttonPrefab, buttonContainer);
            btnObj.GetComponentInChildren<TextMeshProUGUI>().text = choice.choiceText;

            btnObj.GetComponent<Button>().onClick.AddListener(() => {
                if (choice.nextNode != null)
                {
                    DisplayNode(choice.nextNode);
                }
                else
                {
                    EndDialogue();
                }
            });
        }
    }

    // A função crucial para fechar a tela e voltar ao jogo
    public void EndDialogue()
    {
        dialoguePanel.SetActive(false); // Esconde a caixa de diálogo
        Time.timeScale = 1f;            // DESCONGELA O TEMPO!
    }
}