using UnityEngine;
using UnityEngine.UI;
using TMPro;

// ─────────────────────────────────────────────────────────────────────────────
// InventoryHUD — Renderiza a hotbar de 4 slots na parte superior da tela.
//
// COMO CONFIGURAR A HOTBAR NA PARTE SUPERIOR DA TELA:
//
//   1. Crie uma Canvas (Screen Space – Overlay) na cena.
//      • EventSystem é criado automaticamente.
//
//   2. Dentro da Canvas, crie um GameObject vazio chamado "Hotbar".
//      • Adicione o componente Horizontal Layout Group.
//      • RectTransform:
//          – Anchor: Top / Center  (clique na âncora e segure Shift+Alt → topo centro)
//          – Pivot: (0.5, 1)
//          – Pos Y: -10    (margem do topo)
//          – Width: 280    • Height: 70
//      • Horizontal Layout Group:
//          – Spacing: 8
//          – Child Alignment: Middle Center
//          – Control Child Size Width/Height: true
//          – Child Force Expand Width: false
//
//   3. Dentro de "Hotbar", crie 4 filhos chamados "Slot1", "Slot2", "Slot3", "Slot4".
//      Cada slot deve ter:
//        a) Image  → o ícone do item (componente Image)
//        b) Filho "Quantidade" com TextMeshPro - Text (UI)
//           (alinhado canto inferior direito, fonte pequena, ex.: tamanho 14)
//        c) (Opcional) Filho "Tecla" com TextMeshPro exibindo "Z", "X", "C", "V"
//
//   4. Adicione este componente (InventoryHUD) ao GameObject "Hotbar".
//      Atribua no Inspector:
//        • imagensSlots[0..3]    → as 4 Images dos slots
//        • textosQuantidade[0..3] → os 4 TextMeshPro de quantidade
// ─────────────────────────────────────────────────────────────────────────────

public class InventoryHUD : MonoBehaviour
{
    [Header("Slots (arrastar os 4 componentes Image no Inspector)")]
    public Image[] imagensSlots = new Image[InventoryManager.MaxSlots];

    [Header("Textos de Quantidade (arrastar os 4 TextMeshPro no Inspector)")]
    public TextMeshProUGUI[] textosQuantidade = new TextMeshProUGUI[InventoryManager.MaxSlots];

    [Header("Labels de Tecla (opcional — exibe Z/X/C/V em cada slot)")]
    public TextMeshProUGUI[] textosAtalho = new TextMeshProUGUI[InventoryManager.MaxSlots];

    [Header("Visual")]
    [Tooltip("Cor do ícone quando o slot está vazio.")]
    public Color corSlotVazio = new Color(1f, 1f, 1f, 0.15f);

    // Rótulos das teclas exibidos em cada slot
    private static readonly string[] Atalhos = { "Z", "X", "C", "V" };

    // ── ciclo de vida ─────────────────────────────────────────────────────────

    private void OnEnable()
    {
        InventoryManager.OnInventarioAtualizado += AtualizarHUD;
    }

    private void OnDisable()
    {
        InventoryManager.OnInventarioAtualizado -= AtualizarHUD;
    }

    private void Start()
    {
        // Preenche os labels de atalho (Z, X, C, V) — fixos, nunca mudam
        for (int i = 0; i < InventoryManager.MaxSlots; i++)
            if (textosAtalho[i] != null) textosAtalho[i].text = Atalhos[i];

        AtualizarHUD();
    }

    // ── desenho ───────────────────────────────────────────────────────────────

    /// <summary>
    /// Redesenha todos os 4 slots com o estado atual do InventoryManager.
    /// Chamado automaticamente pelo evento OnInventarioAtualizado.
    /// </summary>
    public void AtualizarHUD()
    {
        if (InventoryManager.Instancia == null) return;

        for (int i = 0; i < InventoryManager.MaxSlots; i++)
        {
            var entrada = InventoryManager.Instancia.ObterSlot(i);

            if (entrada != null)
            {
                DesenharSlotPreenchido(i, entrada);
            }
            else
            {
                LimparSlot(i);
            }
        }
    }

    private void DesenharSlotPreenchido(int i, InventoryManager.EntradaInventario entrada)
    {
        if (imagensSlots[i] != null)
        {
            imagensSlots[i].sprite = entrada.itemData.icone;
            imagensSlots[i].color  = entrada.itemData.icone != null ? Color.white : corSlotVazio;
        }

        if (textosQuantidade[i] != null)
        {
            // Exibe a quantidade somente se houver mais de 1 unidade
            textosQuantidade[i].text = entrada.quantidade > 1
                ? entrada.quantidade.ToString()
                : string.Empty;
        }
    }

    private void LimparSlot(int i)
    {
        if (imagensSlots[i] != null)
        {
            imagensSlots[i].sprite = null;
            imagensSlots[i].color  = corSlotVazio;
        }

        if (textosQuantidade[i] != null)
            textosQuantidade[i].text = string.Empty;
    }
}
