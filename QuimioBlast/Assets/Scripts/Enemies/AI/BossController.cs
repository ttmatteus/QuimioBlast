using UnityEngine;

// Boss invisível: estrutura do mapa com vida e ataque à distância.
//
// Como configurar na cena PrimeiraFaseBoss:
//   1. Crie um GameObject vazio sobre a estrutura do mapa (ex: "Boss").
//   2. Adicione: BossController + BoxCollider2D (isTrigger = false) + Rigidbody2D.
//      • Rigidbody2D → Body Type = Kinematic, Constraints = Freeze All.
//      • BoxCollider2D → ajuste o tamanho para cobrir a estrutura visível.
//   3. NÃO adicione SpriteRenderer — o boss é invisível por design.
//   4. Arraste o prefab de projétil no campo "Prefab Projetil".
//      (use o mesmo prefab de projétil dos inimigos, ex: o de EnemyMelee)
//   5. Ajuste dano, velocidade e intervalo de tiro conforme necessário.
//   6. Certifique-se de que haja um VitoriaUI na cena.
//
// Observação sobre paredes (MapBoundary):
//   Os projéteis usam Collider2D com isTrigger = true, então atravessam
//   qualquer colisão estática — inclusive as do MapBoundary. Nenhuma
//   configuração extra é necessária.

[RequireComponent(typeof(Collider2D))]
public class BossController : EnemyBase
{
    [Header("Tiro do Boss")]
    [Tooltip("Prefab do projétil (deve ter Projectile2D, Rigidbody2D e Collider2D isTrigger).")]
    public GameObject prefabProjetil;
    [Tooltip("Velocidade do projétil em unidades/s.")]
    public float velocidadeProjetil = 5f;
    [Tooltip("Dano causado ao player por projétil.")]
    public float danoProjetil = 20f;
    [Tooltip("Segundos entre cada tiro.")]
    public float intervaloTiro = 2f;
    [Tooltip("Alcance máximo do projétil antes de ser destruído.")]
    public float alcanceProjetil = 40f;

    private float timerTiro;
    private Collider2D myCollider;

    protected override void Start()
    {
        base.Start();
        myCollider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (isDead || player == null || prefabProjetil == null) return;

        timerTiro += Time.deltaTime;
        if (timerTiro >= intervaloTiro)
        {
            timerTiro = 0f;
            Atirar();
        }
    }

    private void Atirar()
    {
        Vector2 direcao = ((Vector2)player.position - (Vector2)transform.position).normalized;
        GameObject obj = Instantiate(prefabProjetil, transform.position, Quaternion.identity);
        Projectile2D proj = obj.GetComponent<Projectile2D>();
        if (proj != null)
            proj.Inicializar(direcao, danoProjetil, velocidadeProjetil, alcanceProjetil,
                             myCollider, atingirJogador: true);
    }

    protected override void Die()
    {
        isDead = true;

        foreach (var c in GetComponentsInChildren<Collider2D>())
            c.enabled = false;

        if (VitoriaUI.Instancia != null)
            VitoriaUI.Instancia.Mostrar();
        else
            Debug.LogWarning("[BossController] VitoriaUI não encontrado na cena. Adicione um GameObject com VitoriaUI.");

        Destroy(gameObject, 0.5f);
    }
}
