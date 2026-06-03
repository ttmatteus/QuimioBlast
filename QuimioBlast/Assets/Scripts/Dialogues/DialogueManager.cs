using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections; // Necessário para as Coroutines

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    public GameObject dialoguePanel;
    public TextMeshProUGUI speakerText;
    public TextMeshProUGUI dialogueText;

    [Header("Configurações de Texto")]
    public float typingSpeed = 0.03f; // Velocidade da escrita
    private Coroutine typingCoroutine;

    [Header("Opções")]
    public GameObject buttonPrefab;
    public Transform buttonContainer;

    private void Awake() { Instance = this; }

    public void StartDialogue(DialogueNode startNode)
    {   
        Time.timeScale = 0f; 
        dialoguePanel.SetActive(true);
        DisplayNode(startNode);
    }

    public void DisplayNode(DialogueNode node)
    {
        speakerText.text = node.speakerName;
        
        // Inicia o efeito de máquina de escrever
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeText(node.dialogueText));

        node.onNodeEnter?.Invoke();

        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }

        // Criamos os botões, mas eles podem começar invisíveis ou desativados 
        // se você quiser que o jogador só escolha após o texto terminar.
        if (node.choices == null || node.choices.Length == 0)
        {
            CreateButton("Sair", () => EndDialogue());
        }
        else
        {
            foreach (var choice in node.choices)
            {
                CreateButton(choice.choiceText, () => {
                    if (choice.nextNode != null) DisplayNode(choice.nextNode);
                    else EndDialogue();
                });
            }
        }
    }

    // Coroutine corrigida para funcionar com Time.timeScale = 0
    IEnumerator TypeText(string textToType)
    {
        dialogueText.text = "";
        foreach (char letter in textToType.ToCharArray())
        {
            dialogueText.text += letter;
            // Usamos WaitForSecondsRealtime porque o Time.timeScale está em 0!
            yield return new WaitForSecondsRealtime(typingSpeed);
        }
        typingCoroutine = null;
    }

    // Função auxiliar para criar botões e não repetir código
    private void CreateButton(string text, UnityEngine.Events.UnityAction action)
    {
        GameObject btnObj = Instantiate(buttonPrefab, buttonContainer);
        btnObj.GetComponentInChildren<TextMeshProUGUI>().text = text;
        btnObj.GetComponent<Button>().onClick.AddListener(action);
    }

    public void EndDialogue()
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        dialoguePanel.SetActive(false);
        Time.timeScale = 1f;
    }
}