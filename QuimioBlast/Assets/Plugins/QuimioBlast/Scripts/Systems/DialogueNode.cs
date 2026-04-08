using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class DialogueChoice
{
    public string choiceText; // Texto da opção (Aresta)
    public DialogueNode nextNode; // Nó filho
}

[CreateAssetMenu(fileName = "Novo No", menuName = "QuimioBlast/Dialogue/Node")]
public class DialogueNode : ScriptableObject
{
    public string speakerName; // Interlocutor
    [TextArea(3, 10)]
    public string dialogueText; // Texto do nó

    public DialogueChoice[] choices; // Lista de opções (Arestas para nós filhos)

    [Header("Ação no Jogo")]
    public UnityEvent onNodeEnter; // Executa uma ação (ex: dar item)
}
