using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public abstract class EnemyBase : MonoBehaviour
{
    [Header("vida")]
    public float maxHealth    = 100f;
    protected float currentHealth;

    [Header("detecção")]
    public float detectionRange = 10f;

    [Header("knockback")]
    [Tooltip("Força do recuo leve ao receber dano de Soco, Confringo, Diffindo ou Tiro.")]
    public float forcaKnockback = 4f;

    [Tooltip("Duração do atordoamento causado pelo recuo, em segundos.")]
    public float duracaoKnockback = 0.15f;

    [Header("Drop de Itens")]
    [Tooltip("Prefabs de poção que podem ser dropados ao morrer.")]
    public GameObject[] prefabsDrop;
    [Range(0f, 1f)]
    [Tooltip("Probabilidade de dropar um item (0 = nunca, 1 = sempre).")]
    public float chanceDrop = 0.35f;

    // ── Barra de Vida Flutuante (World Space) ───────────────────────────────────
    [Header("Barra de Vida")]
    [Tooltip("Canvas filha do inimigo configurada em modo World Space.")]
    public Canvas canvasVida;
    [Tooltip("Slider dentro da Canvas que representa a vida atual.")]
    public Slider sliderVida;
    [Tooltip("Distância em unidades acima do centro do inimigo onde a barra aparece.")]
    public float alturaOffsetVida = 1f;

    [Header("indicador de alvo")]
    [SerializeField] private float raioIndicador = 0.6f;
    [SerializeField] private Color corIndicador  = Color.yellow;

    [Header("referência")]
    public Transform player;

    protected bool isDead = false;

    private Rigidbody2D      rbKnockback;
    private SpriteRenderer[] spriteRenderers;
    private Coroutine        flashCoroutine;
    private GameObject       paiIndicador;
    private LineRenderer[]   indicadorAlvo;
    private Material         materialIndicador;

    private MaterialPropertyBlock    flashBlock;
    private static readonly int      ColorId = Shader.PropertyToID("_Color");

    private const float DuracaoFlash = 3 * (0.08f + 0.06f);

    public bool IsDead => isDead;

    protected virtual void Start()
    {
        currentHealth = maxHealth;

        rbKnockback = GetComponent<Rigidbody2D>();

        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        flashBlock = new MaterialPropertyBlock();

        CriarIndicadorAlvo();

        if (canvasVida != null)
        {
            canvasVida.transform.localPosition = new Vector3(0f, alturaOffsetVida, 0f);
            canvasVida.transform.localRotation = Quaternion.identity;
        }

        if (sliderVida != null)
        {
            sliderVida.minValue = 0f;
            sliderVida.maxValue = maxHealth;
            sliderVida.value    = currentHealth;
        }

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
        Shader shader = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default")
                     ?? Shader.Find("Sprites/Default");
        materialIndicador = new Material(shader);

        var srInimigo = GetComponentInChildren<SpriteRenderer>();
        int sortLayer = srInimigo != null ? srInimigo.sortingLayerID : 0;

        paiIndicador = new GameObject("IndicadorMira");
        paiIndicador.transform.SetParent(transform);
        paiIndicador.transform.localPosition = Vector3.zero;
        paiIndicador.AddComponent<RotadorMira>();
        paiIndicador.SetActive(false);

        float s = raioIndicador;
        float l = s * 0.45f;

        Vector3[][] cantos = {
            new[] { new Vector3(-s + l,  s, 0f), new Vector3(-s,  s, 0f), new Vector3(-s,  s - l, 0f) },
            new[] { new Vector3( s - l,  s, 0f), new Vector3( s,  s, 0f), new Vector3( s,  s - l, 0f) },
            new[] { new Vector3( s - l, -s, 0f), new Vector3( s, -s, 0f), new Vector3( s, -s + l, 0f) },
            new[] { new Vector3(-s + l, -s, 0f), new Vector3(-s, -s, 0f), new Vector3(-s, -s + l, 0f) },
        };

        indicadorAlvo = new LineRenderer[4];
        for (int i = 0; i < 4; i++)
        {
            var go = new GameObject($"Canto_{i}");
            go.transform.SetParent(paiIndicador.transform);
            go.transform.localPosition = Vector3.zero;

            var lr = go.AddComponent<LineRenderer>();
            lr.useWorldSpace  = false;
            lr.loop           = false;
            lr.startWidth     = 0.06f;
            lr.endWidth       = 0.06f;
            lr.sortingLayerID = sortLayer;
            lr.sortingOrder   = 10;
            lr.material       = materialIndicador;
            lr.startColor     = corIndicador;
            lr.endColor       = corIndicador;
            lr.positionCount  = 3;
            lr.SetPositions(cantos[i]);

            indicadorAlvo[i] = lr;
        }
    }

    public void MarcarComoAlvo(bool marcado)
    {
        if (paiIndicador != null)
            paiIndicador.SetActive(marcado);
    }

    private void OnDestroy()
    {
        if (materialIndicador != null)
            Destroy(materialIndicador);
    }

    // ── dano / morte ──────────────────────────────────────────────────────────

    public virtual void TakeDamage(float amount)
    {
        if (isDead) return;
        currentHealth -= amount;
        AtualizarSlider();

        if (flashCoroutine != null) StopCoroutine(flashCoroutine);
        flashCoroutine = StartCoroutine(FlashDano());

        if (currentHealth <= 0f) Die();
    }

    public virtual void TakeDamage(float amount, Vector2 origemDano)
    {
        TakeDamage(amount);
        if (isDead || rbKnockback == null) return;

        Vector2 direcao = (Vector2)transform.position - origemDano;
        if (direcao == Vector2.zero) direcao = Vector2.up;
        direcao.Normalize();

        Atordoar(duracaoKnockback);
        rbKnockback.linearVelocity = direcao * forcaKnockback;
    }

    private IEnumerator FlashDano()
    {
        const int   numPiscos   = 3;
        const float tempoVerm   = 0.08f;
        const float tempoNormal = 0.06f;

        for (int i = 0; i < numPiscos; i++)
        {
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

        if (canvasVida != null)
            canvasVida.gameObject.SetActive(false);

        foreach (var c in GetComponentsInChildren<Collider2D>())
            c.enabled = false;

        TentarDrop();
        StartCoroutine(CorotinasMorte());
    }

    private void TentarDrop()
    {
        if (prefabsDrop == null || prefabsDrop.Length == 0) return;
        if (Random.value > chanceDrop) return;

        GameObject prefab = prefabsDrop[Random.Range(0, prefabsDrop.Length)];
        if (prefab != null)
            Instantiate(prefab, transform.position, Quaternion.identity);
    }

    private IEnumerator CorotinasMorte()
    {
        // Aguarda o flash terminar para confirmar a morte visualmente
        if (flashCoroutine != null)
            yield return new WaitForSeconds(DuracaoFlash);

        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
            flashCoroutine = null;
        }
        LimparMPB();

        Destroy(gameObject);
    }

    private void AtualizarSlider()
    {
        if (sliderVida != null)
            sliderVida.value = Mathf.Max(0f, currentHealth);
    }

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

public class RotadorMira : MonoBehaviour
{
    private void Update() => transform.Rotate(0f, 0f, 60f * Time.deltaTime);
}
