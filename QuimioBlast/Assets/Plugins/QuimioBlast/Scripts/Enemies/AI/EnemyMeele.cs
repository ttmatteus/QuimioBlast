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

    // Estados simplificados para a Issue de Aproximação
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

        // Recalcula o caminho apenas se estiver em perseguição
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

        // Garante que o inimigo não rotacione sozinho
        rb.angularVelocity = 0f;
        transform.rotation = Quaternion.identity;
    }

    private void UpdateState()
    {
        float dist = DistanceToPlayer();

        // Lógica de Detecção solicitada na Issue
        if (dist <= detectionRange)
            currentState = State.Chasing;
        else
            currentState = State.Idle;
    }

    private void MoveAlongPath()
    {
        if (path == null || currentWaypoint >= path.vectorPath.Count) return;

        // Direção baseada nos pontos do A* Pathfinding
        Vector2 waypointPos = (Vector2)path.vectorPath[currentWaypoint];
        Vector2 dir = (waypointPos - rb.position).normalized;

        rb.linearVelocity = dir * moveSpeed;

        // Avança para o próximo ponto se estiver perto o suficiente
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