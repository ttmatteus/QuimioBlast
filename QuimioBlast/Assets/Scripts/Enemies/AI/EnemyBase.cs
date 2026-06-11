using UnityEngine;
using System.Collections;

public abstract class EnemyBase : MonoBehaviour
{
    [Header("vida")]
    public float maxHealth    = 100f;
    protected float currentHealth;

    [Header("detecção")]
    public float detectionRange = 10f;

    [Header("renascimento")]
    public float tempoRenascimento = 3f;

    [Header("indicador de alvo")]
    [SerializeField] private float raioIndicador = 0.6f;
    [SerializeField] private Color corIndicador  = Color.yellow;

    [Header("referência")]
    public Transform player;

    protected bool isDead = false;

    private Vector3          posicaoInicial;
    private SpriteRenderer[] spriteRenderers;
    private Color[]          coresOriginais;
    private Coroutine        flashCoroutine;
    private LineRenderer     indicadorAlvo;
    private Material         materialIndicador;

    // MaterialPropertyBlock para o flash — independente do Animator e do material do inimigo
    private MaterialPropertyBlock    flashBlock;
    private static readonly int      ColorId = Shader.PropertyToID("_Color");

    // Duração total do flash (3 piscadas × (0.08 + 0.06))
    private const float DuracaoFlash = 3 * (0.08f + 0.06f);

    public bool IsDead => isDead;

    protected virtual void Start()
    {
        currentHealth  = maxHealth;
        posicaoInicial = transform.position;

        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        coresOriginais  = new Color[spriteRenderers.Length];
        for (int i = 0; i < spriteRenderers.Length; i++)
            coresOriginais[i] = spriteRenderers[i].color;

        flashBlock = new MaterialPropertyBlock();

        CriarIndicadorAlvo();

        if (player == null)
        {
            GameObject found = GameObject.FindGameObjectWithTag("Player");
            if (found != null)
                player = found.transform;
        }
    }

    // ── indicador de alvo ─────────────────────────────────────────────────────

    private void CriarIndicadorAlvo()
    {
        var go = new GameObject("IndicadorAlvo");
        go.transform.SetParent(transform);
        go.transform.localPosition = Vector3.zero;

        indicadorAlvo = go.AddComponent<LineRenderer>();
        indicadorAlvo.useWorldSpace = false;
        indicadorAlvo.loop          = true;
        indicadorAlvo.startWidth    = 0.06f;
        indicadorAlvo.endWidth      = 0.06f;
        indicadorAlvo.sortingOrder  = 10;

        // Tenta o shader URP primeiro, cai no built-in como fallback
        Shader shader = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default")
                     ?? Shader.Find("Sprites/Default");

        materialIndicador      = new Material(shader);
        indicadorAlvo.material = materialIndicador;
        indicadorAlvo.startColor = corIndicador;
        indicadorAlvo.endColor   = corIndicador;

        const int segmentos = 32;
        indicadorAlvo.positionCount = segmentos;
        for (int i = 0; i < segmentos; i++)
        {
            float angulo = i / (float)segmentos * Mathf.PI * 2f;
            indicadorAlvo.SetPosition(i, new Vector3(
                Mathf.Cos(angulo) * raioIndicador,
                Mathf.Sin(angulo) * raioIndicador,
                0f));
        }

        indicadorAlvo.enabled = false;
    }

    public void MarcarComoAlvo(bool marcado)
    {
        if (indicadorAlvo != null)
            indicadorAlvo.enabled = marcado;
    }

    private void OnDestroy()
    {
        if (materialIndicador != null)
            Destroy(materialIndicador);
    }

    // ── dano / morte / renascimento ───────────────────────────────────────────

    public virtual void TakeDamage(float amount)
    {
        if (isDead) return;
        currentHealth -= amount;

        // Reinicia o flash a cada hit
        if (flashCoroutine != null) StopCoroutine(flashCoroutine);
        flashCoroutine = StartCoroutine(FlashDano());

        if (currentHealth <= 0f) Die();
    }

    private IEnumerator FlashDano()
    {
        const int   numPiscos   = 3;
        const float tempoVerm   = 0.08f;
        const float tempoNormal = 0.06f;

        for (int i = 0; i < numPiscos; i++)
        {
            // MaterialPropertyBlock substitui SpriteRenderer.color — funciona mesmo com Animator
            AplicarCorMPB(Color.red);
            yield return new WaitForSeconds(tempoVerm);
            LimparMPB();
            yield return new WaitForSeconds(tempoNormal);
        }

        flashCoroutine = null;
    }

    private void AplicarCorMPB(Color cor)
    {
        flashBlock.Clear();
        flashBlock.SetColor(ColorId, cor);
        foreach (var r in spriteRenderers)
            if (r != null) r.SetPropertyBlock(flashBlock);
    }

    private void LimparMPB()
    {
        foreach (var r in spriteRenderers)
            if (r != null) r.SetPropertyBlock(null);
    }

    public virtual void Atordoar(float duracao)
    {
        StartCoroutine(CorotinaAtordoamento(duracao));
    }

    protected virtual IEnumerator CorotinaAtordoamento(float duracao)
    {
        yield return new WaitForSeconds(duracao);
    }

    protected virtual void Die()
    {
        isDead = true;
        MarcarComoAlvo(false);

        // Desativa colisão imediatamente para não receber mais dano enquanto morre
        foreach (var c in GetComponentsInChildren<Collider2D>())
            c.enabled = false;

        // NÃO cancela o flash — o inimigo pisca e depois some
        StartCoroutine(CorotinaRenascimento());
    }

    private IEnumerator CorotinaRenascimento()
    {
        // Aguarda o flash terminar para que a morte seja visualmente confirmada
        if (flashCoroutine != null)
            yield return new WaitForSeconds(DuracaoFlash);

        // Limpa flash e esconde sprite
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
            flashCoroutine = null;
        }
        LimparMPB();

        foreach (var r in spriteRenderers)
            if (r != null) r.enabled = false;

        yield return new WaitForSeconds(tempoRenascimento);

        transform.position = posicaoInicial;
        currentHealth      = maxHealth;
        isDead             = false;

        foreach (var r in spriteRenderers)
            if (r != null) r.enabled = true;
        foreach (var c in GetComponentsInChildren<Collider2D>())
            c.enabled = true;

        // Garante que não há cor residual de flash ao renascer
        LimparMPB();
        RestaurarCores();
        OnRenascer();
    }

    private void RestaurarCores()
    {
        for (int i = 0; i < spriteRenderers.Length; i++)
            if (spriteRenderers[i] != null) spriteRenderers[i].color = coresOriginais[i];
    }

    protected virtual void OnRenascer() { }

    protected float DistanceToPlayer()
    {
        if (player == null) return Mathf.Infinity;
        return Vector2.Distance(transform.position, player.position);
    }

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
