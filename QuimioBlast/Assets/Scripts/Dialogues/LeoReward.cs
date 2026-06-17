using System.Collections.Generic;
using UnityEngine;

public class LeoReward : MonoBehaviour
{
    public DialogueNode noRecompensa;
    public List<ItemData> itens;

    private bool recompensaDada = false;

    private void OnEnable() => DialogueManager.OnNodeEntered += VerificarRecompensa;
    private void OnDisable() => DialogueManager.OnNodeEntered -= VerificarRecompensa;

    private void VerificarRecompensa(DialogueNode node)
    {
        if (recompensaDada || noRecompensa == null || node != noRecompensa) return;
        recompensaDada = true;
        foreach (var item in itens)
        {
            if (item == null)
            {
                Debug.LogWarning("[LeoReward] Referência de item nula — verifique os campos 'Itens' no Inspector do NPC_Leo.");
                continue;
            }
            InventoryManager.Instancia?.AdicionarItem(item);
        }
    }
}
