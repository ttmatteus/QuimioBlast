using UnityEngine;
using Pathfinding;
using System.Collections;

[RequireComponent(typeof(Seeker))]
[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMelee : EnemyBase
{
    [Header("movimento")]
    public float moveSpeed = 3f;

    private Seeker      seeker;
    private Rigidbody2D rb;
    private Path        path;
    private int         currentWaypoint = 0;

    private float pathUpdateRate = 0.5f;
    private float pathTimer      = 0f;

    private enum State { Idle, Chasing, Stunned }
    private State currentState = State.Idle;

    protected override void Start()
    {
        base.Start();
        seeker = GetComponent<Seeker>();
        rb     = GetComponent<Rigidbody2D>();

        rb.gravityScale  = 0f;
        rb.constraints   = RigidbodyConstraints2D.FreezeRotation;
    }

    private void Update()
    {
        if (isDead || player == null || currentState == State.Stunned) return;

        pathTimer += Time.deltaTime;
        UpdateState();

        if (currentState == State.Chasing && pathTimer >= pathUpdateRate)
        {
            pathTimer = 0f;
            seeker.StartPath(transform.position, player.position, OnPathComplete);
        }
    }

    private void FixedUpdate()
    {
        if (isDead || player == null || currentState == State.Stunned) return;

        if (currentState == State.Chasing)
            MoveAlongPath();
        else
            rb.linearVelocity = Vector2.zero;

        rb.angularVelocity = 0f;
        transform.rotation = Quaternion.identity;
    }

    private void UpdateState()
    {
        // Invisibilidade: para de perseguir enquanto o Player estiver com o efeito ativo
        PlayerHealth ph = player != null ? player.GetComponent<PlayerHealth>() : null;
        if (ph != null && ph.isInvisivel)
        {
            currentState = State.Idle;
            return;
        }

        float dist = DistanceToPlayer();
        currentState = dist <= detectionRange ? State.Chasing : State.Idle;
    }

    private void MoveAlongPath()
    {
        if (path == null || currentWaypoint >= path.vectorPath.Count) return;

        Vector2 waypointPos = (Vector2)path.vectorPath[currentWaypoint];
        Vector2 dir         = (waypointPos - rb.position).normalized;

        rb.linearVelocity = dir * moveSpeed;

        if (Vector2.Distance(rb.position, waypointPos) < 0.2f)
            currentWaypoint++;
    }

    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path            = p;
            currentWaypoint = 0;
        }
    }

    protected override void OnRenascer()
    {
        path            = null;
        currentWaypoint = 0;
        currentState    = State.Idle;
        rb.linearVelocity = Vector2.zero;
    }

    // Aplica atordoamento: para o inimigo pelo tempo informado.
    public override void Atordoar(float duracao)
    {
        StartCoroutine(CorotinaAtordoamento(duracao));
    }

    protected override IEnumerator CorotinaAtordoamento(float duracao)
    {
        State estadoAnterior = currentState;
        currentState         = State.Stunned;
        // Não zeramos velocidade aqui para permitir que knockbacks externos (Accio/Depulso)
        // apliquem movimento enquanto o inimigo está atordoado.

        yield return new WaitForSeconds(duracao);

        rb.linearVelocity = Vector2.zero;
        if (!isDead)
            currentState = estadoAnterior;
    }
}
