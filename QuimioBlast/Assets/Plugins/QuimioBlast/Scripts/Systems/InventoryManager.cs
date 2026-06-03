using UnityEngine;
using System.Collections.Generic;

// ─────────────────────────────────────────────────────────────────────────────
// InventoryManager — Hotbar de 4 slots com compactação automática.
//
// REGRA DE OURO — Compactação:
//   A lista nunca tem buracos. Ao consumir o último exemplar de um item,
//   ele é removido e os itens seguintes deslizam para a esquerda.
//   Exemplo: [Velocidade | Invisibilidade | — | —]
//            ↓ usar Velocidade (última unidade)
//            [Invisibilidade | — | — | —]
//
// TECLAS DE USO:
//   Z = Slot 1 | X = Slot 2 | C = Slot 3 | V = Slot 4
//
// CONFIGURAÇÃO:
//   Adicione este componente ao mesmo GameObject do Player (junto com PlayerHealth).
// ─────────────────────────────────────────────────────────────────────────────

public class InventoryManager : MonoBehaviour
{
    // Singleton: acessível de qualquer lugar via InventoryManager.Instancia
    public static InventoryManager Instancia { get; private set; }

    public const int MaxSlots = 4;

    // ── entrada do inventário ─────────────────────────────────────────────────

    /// <summary>Representa um item e sua quantidade em um slot.</summary>
    public class EntradaInventario
    {
        public ItemData itemData;
        public int quantidade;
    }

    // Lista compacta: sem índices vazios entre itens
    private readonly List<EntradaInventario> slots = new List<EntradaInventario>(MaxSlots);

    // A InventoryHUD escuta este evento para redesenhar a interface
    public System.Action OnInventarioAtualizado;

    private PlayerHealth saude;

    // ── ciclo de vida ─────────────────────────────────────────────────────────

    private void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject);
            return;
        }
        Instancia = this;
    }

    private void Start()
    {
        // Tenta no mesmo GameObject primeiro; se não achar, busca na cena inteira
        saude = GetComponent<PlayerHealth>();
        if (saude == null)
            saude = FindObjectOfType<PlayerHealth>();

        if (saude == null)
            Debug.LogWarning("[InventoryManager] PlayerHealth não encontrado. Adicione-o ao Player.");
    }

    private void Update()
    {
        // Mapeamento rígido: cada tecla dispara o slot correspondente
        if (Input.GetKeyDown(KeyCode.Z)) TentarUsarSlot(0);
        if (Input.GetKeyDown(KeyCode.X)) TentarUsarSlot(1);
        if (Input.GetKeyDown(KeyCode.C)) TentarUsarSlot(2);
        if (Input.GetKeyDown(KeyCode.V)) TentarUsarSlot(3);
    }

    // ── ordenação por prioridade de tipo ─────────────────────────────────────

    // Ordem fixa: CuraTotal → CuraPequena → Velocidade → Invisibilidade
    private static int PrioridadeTipo(TipoEfeito tipo)
    {
        switch (tipo)
        {
            case TipoEfeito.CuraTotal:      return 0;
            case TipoEfeito.CuraPequena:    return 1;
            case TipoEfeito.Velocidade:     return 2;
            case TipoEfeito.Invisibilidade: return 3;
            default:                        return 99;
        }
    }

    // Reordena a lista e notifica a HUD
    private void OrdenarEAtualizar()
    {
        slots.Sort((a, b) =>
            PrioridadeTipo(a.itemData.tipoEfeito)
            .CompareTo(PrioridadeTipo(b.itemData.tipoEfeito)));

        OnInventarioAtualizado?.Invoke();
    }

    // ── interface pública ─────────────────────────────────────────────────────

    /// <summary>
    /// Adiciona um item ao inventário.
    /// Se o item já existir, incrementa a quantidade.
    /// Se for novo, cria um slot (máximo de 4 tipos diferentes).
    /// Após adicionar, a lista é reordenada por prioridade de tipo.
    /// </summary>
    /// <returns>True se o item foi adicionado com sucesso.</returns>
    public bool AdicionarItem(ItemData item)
    {
        // Verifica se este tipo de item já existe e empilha
        EntradaInventario entradaExistente = slots.Find(e => e.itemData == item);
        if (entradaExistente != null)
        {
            entradaExistente.quantidade++;
            Debug.Log($"[InventoryManager] +1 '{item.nomeItem}' (total: {entradaExistente.quantidade})");
            OrdenarEAtualizar();
            return true;
        }

        // Cria novo slot se ainda houver espaço
        if (slots.Count < MaxSlots)
        {
            slots.Add(new EntradaInventario { itemData = item, quantidade = 1 });
            Debug.Log($"[InventoryManager] Novo slot: '{item.nomeItem}'");
            OrdenarEAtualizar();
            return true;
        }

        Debug.Log($"[InventoryManager] Inventário cheio. Não foi possível adicionar '{item.nomeItem}'.");
        return false;
    }

    /// <summary>Retorna a entrada do slot pelo índice (0-based). Null se o slot estiver vazio.</summary>
    public EntradaInventario ObterSlot(int indice)
    {
        if (indice < 0 || indice >= slots.Count) return null;
        return slots[indice];
    }

    /// <summary>Número atual de slots preenchidos (0 a MaxSlots).</summary>
    public int ContarSlots() => slots.Count;

    // ── uso de itens ──────────────────────────────────────────────────────────

    private void TentarUsarSlot(int indice)
    {
        // Tenta recuperar saude caso não tenha sido encontrado no Start
        if (saude == null)
            saude = FindObjectOfType<PlayerHealth>();

        if (saude == null)
        {
            Debug.LogWarning("[InventoryManager] PlayerHealth não encontrado. Verifique se o componente existe na cena.");
            return;
        }

        if (indice >= slots.Count) return; // Slot vazio, nada a fazer

        EntradaInventario entrada = slots[indice];

        if (!entrada.itemData.PodeUsar(saude))
        {
            Debug.Log($"[InventoryManager] '{entrada.itemData.nomeItem}' não pode ser usado agora.");
            return;
        }

        // Aplica o efeito e consome uma unidade
        entrada.itemData.AplicarEfeito(saude);
        entrada.quantidade--;

        if (entrada.quantidade <= 0)
        {
            slots.RemoveAt(indice);
            Debug.Log($"[InventoryManager] Slot {indice + 1} esvaziado. Itens compactados.");
        }

        // Reordena e atualiza a HUD após consumir
        OrdenarEAtualizar();
    }
}
