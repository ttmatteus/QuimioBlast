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

    // ── internos ──────────────────────────────────────────────────────────────
    private Seeker      seeker;
    private Rigidbody2D rb;
    private Camera      cam;

    // WASD
    private Vector2 inputMovimento;
    private bool    estaDashing;
    private float   tempoUltimoClique;
    private KeyCode ultimaTecla;

    // Pathfinding
    private Path caminhoAtual;
    private int  waypointAtual;
    private bool seguindoCaminho;
    private int  tokenCaminho;

    private void Awake()
    {
        seeker = GetComponent<Seeker>();
        rb     = GetComponent<Rigidbody2D>();
        cam    = Camera.main;

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
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        if (x != 0)
            inputMovimento = new Vector2(x, 0);
        else if (y != 0)
            inputMovimento = new Vector2(0, y);
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

    // ── Double-tap Dash ───────────────────────────────────────────────────────

    private void VerificarDoubleTap()
    {
        KeyCode[] teclas = {
            KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D,
            KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow
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
