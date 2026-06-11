using UnityEngine;

// ─────────────────────────────────────────────────────────────────────────────
// CollectibleItem — Item coletável espalhado pelo cenário.
//
// COMO CONFIGURAR UM ITEM NO CENÁRIO:
//   1. Crie um GameObject 2D com SpriteRenderer (sprite do item).
//   2. Adicione um Collider2D (ex.: CircleCollider2D).
//      • Marque "Is Trigger = true".
//   3. Adicione este componente (CollectibleItem).
//   4. No campo "itemData", arraste o ScriptableObject do item desejado
//      (criado via: Criar > QuimioBlast > Item Consumivel).
//   5. Certifique-se de que o GameObject do Player possui a tag "Player".
//
// FLUXO AO COLETAR:
//   Player entra no trigger → CollectibleItem chama InventoryManager.AdicionarItem()
//   → Se houver espaço, o item é adicionado e o objeto do cenário é destruído.
//   → Se o inventário estiver cheio, o objeto permanece no cenário.
// ─────────────────────────────────────────────────────────────────────────────

[RequireComponent(typeof(Collider2D))]
public class CollectibleItem : MonoBehaviour
{
    [Tooltip("ScriptableObject com os dados do item que será coletado.")]
    public ItemData itemData;

    private void Awake()
    {
        // Força o collider a ser sempre um trigger para não bloquear o Player
        Collider2D col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Reage apenas ao Player
        if (!other.CompareTag("Player")) return;

        if (itemData == null)
        {
            Debug.LogWarning($"[CollectibleItem] '{gameObject.name}' não tem ItemData atribuído!");
            return;
        }

        if (InventoryManager.Instancia == null) return;

        bool coletado = InventoryManager.Instancia.AdicionarItem(itemData);

        if (coletado)
        {
            Debug.Log($"[CollectibleItem] '{itemData.nomeItem}' coletado pelo Player.");
            Destroy(gameObject);
        }
        else
        {
            Debug.Log($"[CollectibleItem] Inventário cheio. '{itemData.nomeItem}' não foi coletado.");
        }
    }
}
