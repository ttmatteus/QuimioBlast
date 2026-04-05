using UnityEngine;

// Isso cria uma opção no menu da Unity para você criar novos arquivos de diálogo facilmente
[CreateAssetMenu(fileName = "Novo Dialogo", menuName = "QuimioBlast/Dialogo")]
public class DialogueData : ScriptableObject
{
    [TextArea(3, 10)] // Deixa a caixa de texto maior no Inspector da Unity
    public string[] lines; // Cada posição do array será uma "tela" de fala da ALI
}
