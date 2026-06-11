using UnityEngine;
using System.Collections.Generic;

// Gerenciador de combate do Player.
// Adicione este componente no mesmo GameObject do Player.
//
// Como configurar no Inspector:
//   1. Para cada campo de habilidade, crie o ScriptableObject correspondente
//      (Assets > Create > QuimioBlast > Habilidades > ...) e arraste para o slot.
//   2. Ajuste "Raio Deteccao Inimigos" para cobrir o alcance máximo de busca.
//
// Mapeamento de teclas:
//   - Botão Direito do Mouse → Melee
//   - E                      → Disparo de Energia
//   - 1                      → Incêndio (AoE)
//   - 2                      → Diffindo
//   - 3                      → Accio (puxa inimigo)
//   - 4                      → Depulso (empurra inimigo)

public class CombatManager : MonoBehaviour
{
    [Header("Habilidades")]
    public AbilityBase habilidadeMelee;    // Botão Direito do Mouse
    public AbilityBase habilidadeEnergia;  // E
    public AbilityBase habilidadeIncendio; // 1
    public AbilityBase habilidadeDiffindo; // 2
    public AbilityBase habilidadeAccio;    // 3
    public AbilityBase habilidadeDepulso;  // 4

    [Header("Detecção de Inimigos")]
    [Tooltip("Raio máximo de busca por inimigos ao usar qualquer habilidade.")]
    public float raioDeteccaoInimigos = 15f;

    [Header("Debug")]
    public bool mostrarGizmos = true;

    // ── internos ──────────────────────────────────────────────────────────────
    private Dictionary<AbilityBase, float> cooldowns = new Dictionary<AbilityBase, float>();
    private Animator  animator;
    private Camera    cam;
    private EnemyBase alvoAtual;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        cam      = Camera.main;
    }

    private void Update()
    {
        AtualizarCooldowns();
        AtualizarAlvo();
        LerInputs();
    }

    private void OnDisable()
    {
        // Garante que o indicador seja removido se o CombatManager for desativado
        if (alvoAtual != null)
        {
            alvoAtual.MarcarComoAlvo(false);
            alvoAtual = null;
        }
    }

    private void AtualizarCooldowns()
    {
        var chaves = new List<AbilityBase>(cooldowns.Keys);
        foreach (var habilidade in chaves)
        {
            if (cooldowns[habilidade] > 0f)
                cooldowns[habilidade] -= Time.deltaTime;
        }
    }

    private void AtualizarAlvo()
    {
        EnemyBase novoInimigo = EncontrarInimigoAlvo();

        if (novoInimigo == alvoAtual) return;

        if (alvoAtual != null) alvoAtual.MarcarComoAlvo(false);
        alvoAtual = novoInimigo;
        if (alvoAtual != null) alvoAtual.MarcarComoAlvo(true);
    }

    private void LerInputs()
    {
        if (Input.GetMouseButtonDown(1))           TentarExecutar(habilidadeMelee);
        if (Input.GetKeyDown(KeyCode.E))           TentarExecutar(habilidadeEnergia);
        if (Input.GetKeyDown(KeyCode.Alpha1))      TentarExecutar(habilidadeIncendio);
        if (Input.GetKeyDown(KeyCode.Alpha2))      TentarExecutar(habilidadeDiffindo);
        if (Input.GetKeyDown(KeyCode.Alpha3))      TentarExecutar(habilidadeAccio);
        if (Input.GetKeyDown(KeyCode.Alpha4))      TentarExecutar(habilidadeDepulso);
    }

    private void TentarExecutar(AbilityBase habilidade)
    {
        if (habilidade == null) return;

        if (cooldowns.TryGetValue(habilidade, out float tempoRestante) && tempoRestante > 0f)
        {
            Debug.Log($"[Combate] {habilidade.nomeHabilidade} em cooldown: {tempoRestante:F1}s restantes.");
            return;
        }

        if (alvoAtual == null)
        {
            Debug.Log("[Combate] Nenhum inimigo dentro do raio de detecção na direção atual.");
            return;
        }

        cooldowns[habilidade] = habilidade.cooldown;
        DispararAnimacao(habilidade.parametroAnimacao);
        habilidade.Executar(this, alvoAtual.transform);
    }

    // Encontra o inimigo alvo: prioriza o mais próximo na direção que o cursor aponta.
    // Se nenhum inimigo estiver na metade frontal, cai no mais próximo em geral.
    private EnemyBase EncontrarInimigoAlvo()
    {
        Vector2 direcaoOlhando = ObterDirecaoCursor();
        EnemyBase[] todos = Object.FindObjectsByType<EnemyBase>(FindObjectsSortMode.None);

        EnemyBase maisProximoFrontal  = null;
        float     distFrontal         = raioDeteccaoInimigos;

        EnemyBase maisProximoGeral    = null;
        float     distGeral           = raioDeteccaoInimigos;

        foreach (var inimigo in todos)
        {
            if (inimigo.IsDead) continue;

            float dist = Vector2.Distance(transform.position, inimigo.transform.position);
            if (dist >= raioDeteccaoInimigos) continue;

            // Inimigo mais próximo em geral (fallback)
            if (dist < distGeral)
            {
                distGeral        = dist;
                maisProximoGeral = inimigo;
            }

            // Inimigo na metade frontal (dot > 0 = mesmo lado que o cursor)
            Vector2 dirParaInimigo = ((Vector2)inimigo.transform.position - (Vector2)transform.position).normalized;
            float dot = Vector2.Dot(direcaoOlhando, dirParaInimigo);

            if (dot > 0f && dist < distFrontal)
            {
                distFrontal        = dist;
                maisProximoFrontal = inimigo;
            }
        }

        return maisProximoFrontal != null ? maisProximoFrontal : maisProximoGeral;
    }

    // Retorna a direção normalizada do player ao cursor do mouse no espaço mundo.
    private Vector2 ObterDirecaoCursor()
    {
        if (cam == null) return Vector2.right;
        Vector2 posMouseMundo = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = posMouseMundo - (Vector2)transform.position;
        return dir.magnitude > 0.01f ? dir.normalized : Vector2.right;
    }

    // Mantido público para compatibilidade com outros sistemas que possam chamar.
    public Transform EncontrarInimigoMaisProximo()
    {
        return alvoAtual != null ? alvoAtual.transform : null;
    }

    private void DispararAnimacao(string parametro)
    {
        if (animator == null || string.IsNullOrEmpty(parametro)) return;
        animator.SetTrigger(parametro);
    }

    public float GetCooldownRestante(AbilityBase habilidade)
    {
        if (habilidade == null) return 0f;
        return cooldowns.TryGetValue(habilidade, out float t) ? Mathf.Max(0f, t) : 0f;
    }

    private void OnDrawGizmosSelected()
    {
        if (!mostrarGizmos) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, raioDeteccaoInimigos);

        // Mostra a direção do cursor no editor durante o play
        if (Application.isPlaying && cam != null)
        {
            Gizmos.color = Color.cyan;
            Vector2 dir = ObterDirecaoCursor();
            Gizmos.DrawRay(transform.position, (Vector3)dir * 2f);
        }
    }
}
