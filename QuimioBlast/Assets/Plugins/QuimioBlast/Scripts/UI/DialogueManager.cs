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
        dialoguePanel.SetActive(true);
        DisplayNode(startNode);
    }

    public void DisplayNode(DialogueNode node)
    {
        speakerText.text = node.speakerName;
        dialogueText.text = node.dialogueText;

        // Executa a ação do nó, se houver (Requisito do professor)
        node.onNodeEnter?.Invoke();

        // Limpa botões antigos
        foreach (Transform child in buttonContainer) Destroy(child.gameObject);

        // Cria novos botões para cada opção (Aresta)
        foreach (var choice in node.choices)
        {
            GameObject btnObj = Instantiate(buttonPrefab, buttonContainer);
            btnObj.GetComponentInChildren<TextMeshProUGUI>().text = choice.choiceText;
            btnObj.GetComponent<Button>().onClick.AddListener(() => {
                if (choice.nextNode != null) DisplayNode(choice.nextNode);
                else EndDialogue();
            });
        }
    }

    public void EndDialogue() { dialoguePanel.SetActive(false); }
}