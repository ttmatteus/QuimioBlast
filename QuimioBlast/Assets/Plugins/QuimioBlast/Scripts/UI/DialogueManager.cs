using System.Collections;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    public GameObject dialoguePanel; // O fundo da caixa (RF 14)
    public TextMeshProUGUI dialogueText; // O componente de texto

    private string[] currentLines;
    private int currentLineIndex;
    private bool isTyping;

    private void Awake() { Instance = this; }

    public void StartDialogue(DialogueData dialogue)
    {
        currentLines = dialogue.lines;
        currentLineIndex = 0;
        dialoguePanel.SetActive(true);
        ShowNextLine();
    }

    public void ShowNextLine()
    {
        if (currentLineIndex < currentLines.Length)
        {
            StopAllCoroutines();
            StartCoroutine(TypeSentence(currentLines[currentLineIndex++]));
        }
        else { EndDialogue(); }
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        isTyping = true;
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.03f);
        }
        isTyping = false;
    }

    void Update()
    {
        // Avança diálogo com 'E' ou 'Espaço' (RF 06)
        if (dialoguePanel.activeSelf && (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space)))
        {
            if (!isTyping) ShowNextLine();
            else { StopAllCoroutines(); dialogueText.text = currentLines[currentLineIndex - 1]; isTyping = false; }
        }
    }

    public void EndDialogue() { dialoguePanel.SetActive(false); }
}
