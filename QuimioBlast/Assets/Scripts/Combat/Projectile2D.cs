using UnityEngine;

// Comportamento de projétil 2D.
// Adicione este componente no prefab do projétil junto com:
//   - Rigidbody2D  (gravity = 0)
//   - Collider2D   (Is Trigger = true)
//
// O script é inicializado pelo método Inicializar() chamado pela habilidade.

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile2D : MonoBehaviour
{
    private float   dano;
    private float   distanciaMaxima;
    private Vector3 posicaoInicial;
    private bool    atingirJogador;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb              = GetComponent<Rigidbody2D>();
        rb.bodyType     = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0f;
    }

    // Chamado pela habilidade logo após a instanciação.
    // ownerCollider: collider do dono para ignorar colisão imediata com ele mesmo.
    // atingirJogador: true quando disparado por inimigos — acerta PlayerHealth em vez de EnemyBase.
    public void Inicializar(Vector2 direcao, float dano, float velocidade, float distanciaMaxima, Collider2D ownerCollider = null, bool atingirJogador = false)
    {
        this.dano            = dano;
        this.distanciaMaxima = distanciaMaxima;
        this.atingirJogador  = atingirJogador;
        posicaoInicial       = transform.position;

        rb.linearVelocity = direcao * velocidade;

        float angulo = Mathf.Atan2(direcao.y, direcao.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angulo, Vector3.forward);

        if (ownerCollider != null)
        {
            Collider2D projetilCol = GetComponent<Collider2D>();
            if (projetilCol != null)
                Physics2D.IgnoreCollision(projetilCol, ownerCollider);
        }
    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, posicaoInicial) >= distanciaMaxima)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (atingirJogador)
        {
            PlayerHealth jogador = other.GetComponentInParent<PlayerHealth>();
            if (jogador != null)
            {
                jogador.ReceberDano(dano);
                Destroy(gameObject);
            }
        }
        else
        {
            EnemyBase inimigo = other.GetComponentInParent<EnemyBase>();
            if (inimigo != null)
            {
                inimigo.TakeDamage(dano, transform.position);
                Destroy(gameObject);
            }
        }
    }
}
