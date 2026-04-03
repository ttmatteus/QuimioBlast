using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(Seeker))]
[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMelee : EnemyBase
{
    [Header("movimento")]
    public float moveSpeed = 3f;

    private Seeker seeker;
    private Rigidbody2D rb;
    private Path path;
    private int currentWaypoint = 0;

    private float pathUpdateRate = 0.5f;
    private float pathTimer = 0f;

    // estados básicos que preciso pra aproximação
    private enum State { Idle, Chasing }
    private State currentState = State.Idle;

    protected override void Start()
    {
        base.Start();
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void Update()
    {
        if (isDead || player == null) return;

        pathTimer += Time.deltaTime;

        UpdateState();

        // só recalcula caminho quando realmente estiver perseguindo
        if (currentState == State.Chasing && pathTimer >= pathUpdateRate)
        {
            pathTimer = 0f;
            seeker.StartPath(transform.position, player.position, OnPathComplete);
        }
    }

    private void FixedUpdate()
    {
        if (isDead || player == null) return;

        if (currentState == State.Chasing)
        {
            MoveAlongPath();
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }

        // garante que o inimigo nunca rotacione sozinho
        rb.angularVelocity = 0f;
        transform.rotation = Quaternion.identity;
    }

    private void UpdateState()
    {
        float dist = DistanceToPlayer();

        // lógica simples de detecção
        if (dist <= detectionRange)
            currentState = State.Chasing;
        else
            currentState = State.Idle;
    }

    private void MoveAlongPath()
    {
        if (path == null || currentWaypoint >= path.vectorPath.Count) return;

        // direção com base nos pontos do a*
        Vector2 waypointPos = (Vector2)path.vectorPath[currentWaypoint];
        Vector2 dir = (waypointPos - rb.position).normalized;

        rb.linearVelocity = dir * moveSpeed;

        // troca pro próximo ponto quando estiver perto o suficiente
        if (Vector2.Distance(rb.position, waypointPos) < 0.2f)
        {
            currentWaypoint++;
        }
    }

    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }
}