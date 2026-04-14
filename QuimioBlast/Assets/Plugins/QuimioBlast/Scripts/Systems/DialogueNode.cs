using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class DialogueChoice
{
    public string choiceText; // Texto da op��o (Aresta)
    public DialogueNode nextNode; // N� filho
}

[CreateAssetMenu(fileName = "Novo No", menuName = "QuimioBlast/Dialogue/Node")]
public class DialogueNode : ScriptableObject
{
    public string speakerName; // Interlocutor
    [TextArea(3, 10)]
    public string dialogueText; // Texto do n�

    public DialogueChoice[] choices; // Lista de op��es (Arestas para n�s filhos)

    [Header("A��o no Jogo")]
    public UnityEvent onNodeEnter; // Executa uma a��o (ex: dar item)
}
