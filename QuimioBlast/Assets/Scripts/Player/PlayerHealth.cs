using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// ─────────────────────────────────────────────────────────────────────────────
// PlayerHealth — vida do Player, dano por contato e barra de vida flutuante.
//
// COMO CONFIGURAR A BARRA DE VIDA FLUTUANTE (World Space):
//   1. Selecione o GameObject do Player na Hierarquia.
//   2. Clique com botão direito > UI > Canvas  (cria uma Canvas FILHA do Player).
//   3. No Inspector da Canvas:
//      • Render Mode  → World Space
//      • Width = 1    • Height = 0.15   • Scale X/Y/Z = 0.01
//      • Posição Y    → mesmo valor de "alturaOffset" (padrão: 1)
//   4. Dentro da Canvas, adicione: UI > Slider
//      • Remova "Handle Slide Area" (objeto filho) para visual limpo.
//      • Slider > Min Value = 0  |  Max Value = 1  |  Whole Numbers = false
//      • Desative a interação: Interactable = false
//   5. Arraste a Canvas para o campo "Canvas Vida" e o Slider para "Slider Vida".
//
// DANO POR CONTATO:
//   • O Player recebe dano ao colidir com qualquer objeto com a tag "Enemy".
//   • Há um cooldown de imunidade para evitar morte instantânea.
//
// EFEITOS TEMPORÁRIOS (chamados pelo InventoryManager):
//   • AumentarVelocidade — aumenta velocidadeBase do PlayerMovement2D.
//   • AtivarInvisibilidade — ativa flag que os inimigos checam para parar de perseguir.
// ─────────────────────────────────────────────────────────────────────────────

[RequireComponent(typeof(Collider2D))]
public class PlayerHealth : MonoBehaviour
{
    [Header("Vida")]
    public float vidaMaxima = 100f;

    [Header("Dano por Contato com Inimigo")]
    [Tooltip("Quantidade de dano recebido ao encostar em um inimigo.")]
    public float danoInimigo = 20f;
    [Tooltip("Segundos de imunidade após receber dano (evita morte instantânea).")]
    public float cooldownImunidade = 1f;

    [Header("Barra de Vida Flutuante (World Space)")]
    [Tooltip("Canvas filha do Player configurada em modo World Space.")]
    public Canvas canvasVida;
    [Tooltip("Slider dentro da Canvas que representa a vida atual.")]
    public Slider sliderVida;
    [Tooltip("Distância em unidades acima do centro do Player onde a barra aparece.")]
    public float alturaOffset = 1f;

    // NonSerialized impede que Unity salve 0 na cena e sobrescreva o Awake
    [System.NonSerialized] public float vidaAtual;

    // Flag de invisibilidade checada pelos inimigos
    [HideInInspector] public bool isInvisivel = false;

    // Eventos para outros scripts reagirem a mudanças de vida
    public System.Action<float, float> OnVidaMudou; // (vidaAtual, vidaMaxima)
    public System.Action OnMorreu;

    private bool estaImune = false;
    private bool velocidadeAumentada = false;
    private PlayerMovement2D movimento;
    private Camera camPrincipal;

    // ── ciclo de vida ─────────────────────────────────────────────────────────

    private void Awake()
    {
        vidaAtual    = vidaMaxima;
        movimento    = GetComponent<PlayerMovement2D>();
        camPrincipal = Camera.main;

        if (canvasVida != null)
        {
            canvasVida.transform.localPosition = new Vector3(0f, alturaOffset, 0f);
            // Mantém a Canvas no plano XY — câmera 2D ortográfica sempre enxerga de frente
            canvasVida.transform.localRotation = Quaternion.identity;
        }

        if (sliderVida != null)
        {
            sliderVida.minValue = 0f;
            sliderVida.maxValue = vidaMaxima;
            sliderVida.value    = vidaMaxima;
        }

        Debug.Log($"[PlayerHealth] Inicializado — vidaAtual={vidaAtual} vidaMaxima={vidaMaxima} sliderVida={(sliderVida != null ? "OK" : "NULL — arraste o Slider no Inspector!")}");
    }

    private void Start()
    {
        AtualizarSlider();
    }

    private void LateUpdate()
    {
        // Em 2D com câmera ortográfica a Canvas não precisa rotacionar.
        // Apenas garante que nunca fique inclinada caso algo externo altere a rotação.
        if (canvasVida != null)
            canvasVida.transform.rotation = Quaternion.identity;
    }

    // ── dano e cura ──────────────────────────────────────────────────────────

    /// <summary>Aplica dano ao Player. Ignora se estiver imune ou já morto.</summary>
    public void ReceberDano(float quantidade)
    {
        if (estaImune || vidaAtual <= 0f) return;

        vidaAtual = Mathf.Max(0f, vidaAtual - quantidade);
        AtualizarSlider();
        OnVidaMudou?.Invoke(vidaAtual, vidaMaxima);

        if (vidaAtual <= 0f)
        {
            OnMorreu?.Invoke();
            Debug.Log("[PlayerHealth] O Player morreu.");
        }
        else
        {
            StartCoroutine(CorotinImunidade());
        }
    }

    /// <summary>Cura uma quantidade fixa, respeitando o limite máximo.</summary>
    public void Curar(float quantidade)
    {
        if (vidaAtual >= vidaMaxima) return;
        vidaAtual = Mathf.Min(vidaMaxima, vidaAtual + quantidade);
        AtualizarSlider();
        OnVidaMudou?.Invoke(vidaAtual, vidaMaxima);
    }

    /// <summary>Restaura toda a vida. Bloqueado se já estiver com vida cheia.</summary>
    public void CurarTotal()
    {
        if (vidaAtual >= vidaMaxima) return;
        vidaAtual = vidaMaxima;
        AtualizarSlider();
        OnVidaMudou?.Invoke(vidaAtual, vidaMaxima);
    }

    // ── efeitos temporários (usados pelo ItemData) ────────────────────────────

    /// <summary>
    /// Aumenta a velocidade base do Player por uma duração.
    /// Bloqueado se um buff de velocidade já estiver ativo (sem empilhamento).
    /// </summary>
    /// <param name="multiplicador">Ex.: 1.5 para +50%.</param>
    /// <param name="duracao">Duração em segundos.</param>
    public void AumentarVelocidade(float multiplicador, float duracao)
    {
        if (velocidadeAumentada) return;
        StartCoroutine(CorotinVelocidade(multiplicador, duracao));
    }

    /// <summary>
    /// Torna o Player invisível para a IA dos inimigos durante a duração informada.
    /// </summary>
    public void AtivarInvisibilidade(float duracao)
    {
        StartCoroutine(CorotinInvisibilidade(duracao));
    }

    // ── detecção de colisão com inimigos ─────────────────────────────────────

    // Cobre cenário onde o inimigo tem Collider físico (não-trigger)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
            ReceberDano(danoInimigo);
    }

    // Cobre cenário onde o inimigo tem Collider do tipo Trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
            ReceberDano(danoInimigo);
    }

    // ── coroutines internas ───────────────────────────────────────────────────

    private IEnumerator CorotinImunidade()
    {
        estaImune = true;
        yield return new WaitForSeconds(cooldownImunidade);
        estaImune = false;
    }

    private IEnumerator CorotinVelocidade(float multiplicador, float duracao)
    {
        if (movimento == null) yield break;

        velocidadeAumentada = true;
        float velocidadeOriginal = movimento.velocidadeBase;
        movimento.velocidadeBase *= multiplicador;

        yield return new WaitForSeconds(duracao);

        movimento.velocidadeBase = velocidadeOriginal;
        velocidadeAumentada = false;
    }

    private IEnumerator CorotinInvisibilidade(float duracao)
    {
        isInvisivel = true;
        yield return new WaitForSeconds(duracao);
        isInvisivel = false;
    }

    public bool IsFullHealth() => vidaAtual >= vidaMaxima;
    public void Heal(float quantidade) => Curar(quantidade);

    // ── utilidade ─────────────────────────────────────────────────────────────

    private void AtualizarSlider()
    {
        if (sliderVida != null)
            sliderVida.value = vidaAtual; // Escala direta: slider.max = vidaMaxima
    }
}
