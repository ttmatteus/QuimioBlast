using UnityEngine;

// ─────────────────────────────────────────────────────────────────────────────
// ItemData — ScriptableObject que define um item consumível.
//
// COMO CRIAR UM NOVO ITEM:
//   1. No Project, clique com botão direito em qualquer pasta.
//   2. Criar > QuimioBlast > Item Consumivel
//   3. Configure os campos no Inspector:
//      • nomeItem   → nome exibido na hotbar
//      • icone      → Sprite que aparece no slot
//      • tipoEfeito → escolha um dos 4 tipos abaixo
//      • valorEfeito:
//          - CuraPequena:   quantidade de vida restaurada (ex.: 20)
//          - Velocidade:    porcentagem de aumento (ex.: 50 = +50%)
//          - CuraTotal/Invisibilidade: campo ignorado
//      • duracaoEfeito: segundos de duração (apenas Velocidade e Invisibilidade)
//
// TIPOS DISPONÍVEIS (TipoEfeito):
//   CuraTotal       → restaura 100% da vida (bloqueado se vida cheia)
//   CuraPequena     → cura um valor fixo (bloqueado se vida cheia)
//   Velocidade      → aumenta velocidade base em X% por N segundos
//   Invisibilidade  → inimigos ignoram o Player por N segundos
// ─────────────────────────────────────────────────────────────────────────────

/// <summary>Tipo de efeito aplicado ao usar o item.</summary>
public enum TipoEfeito
{
    CuraTotal,
    CuraPequena,
    Velocidade,
    Invisibilidade
}

[CreateAssetMenu(fileName = "NovoItem", menuName = "QuimioBlast/Item Consumivel")]
public class ItemData : ScriptableObject
{
    [Header("Identificação")]
    public string nomeItem = "Item sem nome";
    public Sprite icone;

    [Header("Efeito")]
    public TipoEfeito tipoEfeito;

    [Tooltip("Cura fixa (CuraPequena) ou % de aumento de velocidade (Velocidade). Ignorado nos outros tipos.")]
    public float valorEfeito = 20f;

    [Tooltip("Duração em segundos — apenas para Velocidade e Invisibilidade.")]
    public float duracaoEfeito = 10f;

    // ── validação de uso ──────────────────────────────────────────────────────

    /// <summary>
    /// Retorna true se o item pode ser usado no momento.
    /// Curas são bloqueadas quando a vida já está no máximo.
    /// </summary>
    public bool PodeUsar(PlayerHealth saude)
    {
        return true;
    }

    // ── aplicação do efeito ───────────────────────────────────────────────────

    /// <summary>
    /// Aplica o efeito deste item sobre o Player.
    /// Chamado pelo InventoryManager após validar PodeUsar().
    /// </summary>
    public void AplicarEfeito(PlayerHealth saude)
    {
        switch (tipoEfeito)
        {
            case TipoEfeito.CuraTotal:
                saude.CurarTotal();
                break;

            case TipoEfeito.CuraPequena:
                saude.Curar(valorEfeito);
                break;

            case TipoEfeito.Velocidade:
                // valorEfeito = 50 → multiplicador = 1.5 (+50%)
                float multiplicador = 1f + valorEfeito / 100f;
                saude.AumentarVelocidade(multiplicador, duracaoEfeito);
                break;

            case TipoEfeito.Invisibilidade:
                saude.AtivarInvisibilidade(duracaoEfeito);
                break;
        }

        Debug.Log($"[ItemData] Efeito '{tipoEfeito}' do item '{nomeItem}' aplicado.");
    }
}
