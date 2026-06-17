using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMelee : EnemyBase
{
    [Header("movimento")]
    public float moveSpeed = 3f;

    [Header("Ataque à Distância (opcional — deixe vazio para desativar)")]
    public GameObject prefabProjetil;
    public float velocidadeProjetil = 6f;
    public float danoProjetil       = 15f;
    public float intervaloTiro      = 2.5f;
    public float alcanceTiro        = 8f;

    private Rigidbody2D    rb;
    private SpriteRenderer sr;
    private float          timerTiro = 0f;

    private enum State { Idle, Chasing, Stunned }
    private State currentState = State.Idle;

    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        rb.gravityScale = 0f;
        rb.constraints  = RigidbodyConstraints2D.FreezeRotation;
    }

    private void Update()
    {
        if (isDead || player == null || currentState == State.Stunned) return;

        UpdateState();

        if (prefabProjetil != null)
            AtualizarTiro();
    }

    private void FixedUpdate()
    {
        if (isDead || player == null || currentState == State.Stunned) return;

        if (currentState == State.Chasing)
            SeguirJogador();
        else
            rb.linearVelocity = Vector2.zero;

        rb.angularVelocity = 0f;
        transform.rotation = Quaternion.identity;

        FlipParaJogador();
    }

    private void UpdateState()
    {
        PlayerHealth ph = player != null ? player.GetComponent<PlayerHealth>() : null;
        if (ph != null && ph.isInvisivel)
        {
            currentState = State.Idle;
            return;
        }

        float dist = DistanceToPlayer();
        currentState = dist <= detectionRange ? State.Chasing : State.Idle;
    }

    private void SeguirJogador()
    {
        if (player == null) return;
        Vector2 dir = ((Vector2)player.position - rb.position).normalized;
        rb.linearVelocity = dir * moveSpeed;
    }

    private void FlipParaJogador()
    {
        if (sr == null || player == null) return;
        sr.flipX = player.position.x < transform.position.x;
    }

    private void AtualizarTiro()
    {
        if (isDead || currentState != State.Chasing) return;
        if (DistanceToPlayer() > alcanceTiro) return;

        timerTiro += Time.deltaTime;
        if (timerTiro < intervaloTiro) return;

        timerTiro = 0f;
        Atirar();
    }

    private void Atirar()
    {
        Vector2 direcao = ((Vector2)player.position - (Vector2)transform.position).normalized;
        GameObject obj  = Instantiate(prefabProjetil, transform.position, Quaternion.identity);
        Projectile2D proj = obj.GetComponent<Projectile2D>();
        if (proj != null)
            proj.Inicializar(direcao, danoProjetil, velocidadeProjetil, alcanceTiro,
                             GetComponent<Collider2D>(), atingirJogador: true);
    }

    public override void Atordoar(float duracao)
    {
        StartCoroutine(CorotinaAtordoamento(duracao));
    }

    protected override IEnumerator CorotinaAtordoamento(float duracao)
    {
        State estadoAnterior = currentState;
        currentState         = State.Stunned;

        yield return new WaitForSeconds(duracao);

        rb.linearVelocity = Vector2.zero;
        if (!isDead)
            currentState = estadoAnterior;
    }
}
