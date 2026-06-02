using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Configurações de Ataque")]
    public float dano = 25f;
    public float alcance = 1.2f;
    public float cooldown = 0.5f;
    public LayerMask camadaInimigos = ~0;

    [Header("Duração da Hitbox")]
    public float duracaoHitbox = 0.15f;

    private PlayerMovement2D movimento;
    private float tempoCooldown;
    private bool estaAtacando;

    private void Awake()
    {
        movimento = GetComponent<PlayerMovement2D>();
    }

    private void Update()
    {
        tempoCooldown -= Time.deltaTime;

        if (Input.GetMouseButtonDown(0) && tempoCooldown <= 0f && !estaAtacando)
            StartCoroutine(ExecutarAtaque());
    }

    private IEnumerator ExecutarAtaque()
    {
        estaAtacando = true;
        tempoCooldown = cooldown;

        // Espera um frame para parecer responsivo antes de aplicar dano
        yield return null;

        AplicarDano();

        yield return new WaitForSeconds(duracaoHitbox);

        estaAtacando = false;
    }

    private void AplicarDano()
    {
        Vector2 origem = (Vector2)transform.position + movimento.UltimaDirecao * (alcance * 0.5f);
        Collider2D[] atingidos = Physics2D.OverlapCircleAll(origem, alcance * 0.5f, camadaInimigos);

        foreach (Collider2D col in atingidos)
        {
            EnemyBase inimigo = col.GetComponent<EnemyBase>();
            if (inimigo != null)
                inimigo.TakeDamage(dano);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (movimento == null) return;

        Vector2 origem = (Vector2)transform.position + movimento.UltimaDirecao * (alcance * 0.5f);
        Gizmos.color = estaAtacando ? Color.red : new Color(1f, 0.4f, 0f, 0.5f);
        Gizmos.DrawWireSphere(origem, alcance * 0.5f);
    }
}
