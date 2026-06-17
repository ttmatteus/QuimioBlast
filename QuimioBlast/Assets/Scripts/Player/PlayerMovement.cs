using UnityEngine;
using UnityEngine.EventSystems;
using Pathfinding;
using System.Collections;

// Movimento do Player: WASD (direto) + Clique Esquerdo (Click-to-Move via A*).
// Os dois modos coexistem — pressionar WASD cancela o caminho atual; clicar
// cancela o input de teclado e inicia o pathfinding.
//
// Componentes obrigatórios no mesmo GameObject:
//   - Seeker        (A* Pathfinding Project)
//   - Rigidbody2D   (gravity = 0, Freeze Rotation Z)
//
// Configuração no Inspector:
//   • Velocidade Base / Multiplicador Sprint / Forca Dash → ajuste a gosto
//   • Prefab Marcador → ícone que aparece onde o jogador clicou (opcional)
//   • Double-tap numa tecla direcional ainda executa o Dash

[RequireComponent(typeof(Seeker))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerMovement2D : MonoBehaviour
{
    [Header("Velocidade")]
    public float velocidadeBase       = 7f;
    public float multiplicadorSprint  = 1.5f;

    [Header("Dash")]
    public float forcaDash          = 20f;
    public float tempoDash          = 0.2f;
    public float intervaloDoubleTap = 0.3f;

    [Header("Pathfinding (Click-to-Move)")]
    [Tooltip("Distância para considerar que chegou a um waypoint.")]
    public float distanciaChegada = 0.25f;

    [Header("Feedback Visual")]
    [Tooltip("Prefab instantâneo exibido onde o jogador clicou (opcional).")]
    public GameObject prefabMarcador;

    // ── animação ─────────────────────────────────────────────────────────────
    // IDs dos estados — os valores devem ser IGUAIS aos usados nas transições
    // "Any State" do Animator Controller (parâmetro Int "Estado").
    private const int EstadoAndandoFrente = 0; // S      → anda para baixo
    private const int EstadoAndandoCosta  = 1; // W      → anda para cima
    private const int EstadoAndandoLado   = 2; // A / D  → anda de lado (flipX inverte para a esquerda)
    private const int EstadoParado        = 3; // idle — última direção foi A, S ou D
    private const int EstadoParadoCostas  = 4; // idle — última direção foi W

    // StringToHash evita comparar strings a cada frame (mais performático)
    private static readonly int ParamEstado   = Animator.StringToHash("Estado");
    private static readonly int ParamAtacando = Animator.StringToHash("Atacando");

    // ── internos ──────────────────────────────────────────────────────────────
    private Seeker        seeker;
    private Rigidbody2D   rb;
    private Camera        cam;
    private Animator      animator;
    private SpriteRenderer spriteRenderer;

    // WASD
    private Vector2 inputMovimento;
    private bool    estaDashing;
    private float   tempoUltimoClique;
    private KeyCode ultimaTecla;

    public Vector2 UltimaDirecao { get; private set; } = Vector2.down;

    // Pathfinding
    private Path caminhoAtual;
    private int  waypointAtual;
    private bool seguindoCaminho;
    private int  tokenCaminho;

    private void Awake()
    {
        seeker         = GetComponent<Seeker>();
        rb             = GetComponent<Rigidbody2D>();
        cam            = Camera.main;
        animator       = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        rb.gravityScale = 0f;
        rb.constraints  = RigidbodyConstraints2D.FreezeRotation;
    }

    private void Update()
    {
        if (estaDashing) return;

        ProcessarInputWASD();
        VerificarDoubleTap();

        // Clique esquerdo inicia o pathfinding
        if (Input.GetMouseButtonDown(0) && !CliqueNaUI())
            ProcessarCliqueMouse();

        // Qualquer tecla direcional cancela o caminho atual
        if (inputMovimento != Vector2.zero && seguindoCaminho)
            CancelarCaminho();

        AtualizarAnimacao();
    }

    private void FixedUpdate()
    {
        if (estaDashing) return;

        if (seguindoCaminho)
            SeguirCaminho();         // modo pathfinding
        else
            MoverWASD();             // modo teclado

        rb.angularVelocity = 0f;
    }

    // ── WASD ─────────────────────────────────────────────────────────────────

    private void ProcessarInputWASD()
    {
        // Teclas explícitas (NÃO usar Input.GetAxisRaw): por padrão os eixos
        // "Horizontal"/"Vertical" do Input Manager também respondem às setas,
        // e as setas agora são reservadas para os golpes (CombatManager).
        float x = 0f;
        float y = 0f;

        if (Input.GetKey(KeyCode.D)) x += 1f;
        if (Input.GetKey(KeyCode.A)) x -= 1f;
        if (Input.GetKey(KeyCode.W)) y += 1f;
        if (Input.GetKey(KeyCode.S)) y -= 1f;

        if (x != 0)
        {
            inputMovimento = new Vector2(x, 0);
            UltimaDirecao = inputMovimento;
        }
        else if (y != 0)
        {
            inputMovimento = new Vector2(0, y);
            UltimaDirecao = inputMovimento;
        }
        else
            inputMovimento = Vector2.zero;
    }

    private void MoverWASD()
    {
        if (inputMovimento == Vector2.zero)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        float velocidade = velocidadeBase;
        if (Input.GetKey(KeyCode.LeftShift))
            velocidade *= multiplicadorSprint;

        rb.linearVelocity = inputMovimento.normalized * velocidade;
    }

    // ── Animação ─────────────────────────────────────────────────────────────

    // Decide qual animação tocar de acordo com a direção atual de movimento e,
    // se o personagem estiver parado, com a última direção registrada em
    // "UltimaDirecao" (atualizada em ProcessarInputWASD a cada tecla W/A/S/D).
    private void AtualizarAnimacao()
    {
        // Prioriza o input de teclado deste frame; se não houver (ex.: durante
        // o click-to-move), usa a velocidade atual do Rigidbody2D — assim a
        // animação também acompanha o modo "andar até o clique".
        Vector2 direcao = inputMovimento != Vector2.zero
            ? inputMovimento
            : rb.linearVelocity;

        bool movendo = direcao.sqrMagnitude > 0.01f;

        if (movendo)
        {
            if (Mathf.Abs(direcao.x) > Mathf.Abs(direcao.y))
            {
                // Movimento horizontal: "AndandoLado" serve para A e D —
                // o flipX espelha o sprite quando a direção é para a esquerda.
                animator.SetInteger(ParamEstado, EstadoAndandoLado);
                spriteRenderer.flipX = direcao.x < 0f;
            }
            else if (direcao.y > 0f)
            {
                animator.SetInteger(ParamEstado, EstadoAndandoCosta);  // W
            }
            else
            {
                animator.SetInteger(ParamEstado, EstadoAndandoFrente); // S
            }
        }
        else
        {
            // Parado: a pose depende de qual tecla direcional foi pressionada
            // por último. "flipX" não é alterado aqui — mantém o espelhamento
            // de quando o personagem estava andando de lado.
            bool ultimaFoiParaCima = UltimaDirecao.y > 0f
                                  && Mathf.Abs(UltimaDirecao.y) > Mathf.Abs(UltimaDirecao.x);

            animator.SetInteger(ParamEstado, ultimaFoiParaCima ? EstadoParadoCostas : EstadoParado);
        }
    }

    // Chamado por um Animation Event no ÚLTIMO frame de cada animação de
    // ataque (Soco, Tiro, Accio, Depulso, Diffindo, Confringo). Libera o bool
    // "Atacando" para que as transições de movimento voltem a funcionar.
    public void FinalizarAtaque()
    {
        animator.SetBool(ParamAtacando, false);
    }

    // ── Double-tap Dash ───────────────────────────────────────────────────────

    private void VerificarDoubleTap()
    {
        // Setas removidas do double-tap: agora são teclas exclusivas de combate.
        KeyCode[] teclas = {
            KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D
        };

        foreach (KeyCode tecla in teclas)
        {
            if (!Input.GetKeyDown(tecla)) continue;

            float delta = Time.time - tempoUltimoClique;
            if (tecla == ultimaTecla && delta < intervaloDoubleTap)
                StartCoroutine(ExecutarDash());

            ultimaTecla       = tecla;
            tempoUltimoClique = Time.time;
        }
    }

    private System.Collections.IEnumerator ExecutarDash()
    {
        estaDashing = true;
        CancelarCaminho();

        Vector2 dir = inputMovimento != Vector2.zero
            ? inputMovimento
            : ObterDirecaoPorTecla(ultimaTecla);

        rb.linearVelocity = dir * forcaDash;

        yield return new WaitForSeconds(tempoDash);

        estaDashing = false;
    }

    private Vector2 ObterDirecaoPorTecla(KeyCode tecla)
    {
        switch (tecla)
        {
            case KeyCode.W: case KeyCode.UpArrow:    return Vector2.up;
            case KeyCode.S: case KeyCode.DownArrow:  return Vector2.down;
            case KeyCode.A: case KeyCode.LeftArrow:  return Vector2.left;
            case KeyCode.D: case KeyCode.RightArrow: return Vector2.right;
            default:                                  return Vector2.zero;
        }
    }

    // ── Click-to-Move ─────────────────────────────────────────────────────────

    private void ProcessarCliqueMouse()
    {
        Vector3 posicaoMundo = cam.ScreenToWorldPoint(Input.mousePosition);
        posicaoMundo.z = 0f;

        MostrarMarcador(posicaoMundo);

        // Incrementa o token para invalidar callbacks pendentes do seeker
        tokenCaminho++;
        int token = tokenCaminho;
        seeker.StartPath(transform.position, posicaoMundo, p => AoCompletarCaminho(p, token));
    }

    private void AoCompletarCaminho(Path p, int token)
    {
        if (p.error)
        {
            Debug.LogWarning("[PlayerMovement] A* não encontrou caminho: " + p.errorLog);
            return;
        }

        // Ignora callbacks de caminhos cancelados (token desatualizado)
        if (token != tokenCaminho) return;

        caminhoAtual    = p;
        waypointAtual   = 0;
        seguindoCaminho = true;
    }

    private void SeguirCaminho()
    {
        if (caminhoAtual == null || waypointAtual >= caminhoAtual.vectorPath.Count)
        {
            CancelarCaminho();
            return;
        }

        // Avança waypoints ANTES de definir velocidade para evitar direção obsoleta
        while (waypointAtual < caminhoAtual.vectorPath.Count &&
               Vector2.Distance(rb.position, (Vector2)caminhoAtual.vectorPath[waypointAtual]) < distanciaChegada)
        {
            waypointAtual++;
        }

        // Para imediatamente ao esgotar waypoints (sem frame extra de movimento)
        if (waypointAtual >= caminhoAtual.vectorPath.Count)
        {
            CancelarCaminho();
            return;
        }

        Vector2 destino = (Vector2)caminhoAtual.vectorPath[waypointAtual];
        Vector2 direcao = (destino - rb.position).normalized;
        rb.linearVelocity = direcao * velocidadeBase;
    }

    private void CancelarCaminho()
    {
        tokenCaminho++;   // invalida qualquer callback pendente do seeker
        seguindoCaminho   = false;
        caminhoAtual      = null;
        rb.linearVelocity = Vector2.zero;

        // Aborta o cálculo em andamento na thread do A*. Sem isso, um path
        // ainda sendo processado mantém a thread de pathfinding ocupada e,
        // ao recarregar a cena (Game Over > Tentar Novamente), o AstarPath
        // antigo trava o thread principal em OnDestroy esperando essa thread
        // terminar — congelando o Editor sem nenhum erro no Console.
        seeker.CancelCurrentPathRequest();
    }

    private void MostrarMarcador(Vector3 posicao)
    {
        if (prefabMarcador == null) return;
        GameObject marcador = Instantiate(prefabMarcador, posicao, Quaternion.identity);
        Destroy(marcador, 1f);
    }

    // ── utilidades ────────────────────────────────────────────────────────────

    // Para toda a movimentação (útil em diálogos, cinemáticas etc.).
    public void PararMovimento()
    {
        CancelarCaminho();
        inputMovimento    = Vector2.zero;
        rb.linearVelocity = Vector2.zero;
    }

    private static bool CliqueNaUI()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }
}
