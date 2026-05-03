
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    [Header("vida")]
    public float maxHealth = 100f;
    protected float currentHealth;

    [Header("detecção")]
    public float detectionRange = 10f;

    [Header("referência")]
    public Transform player;

    protected bool isDead = false;

    protected virtual void Start()
    {
        currentHealth = maxHealth;

        // se o player não for arrastado no inspector, tenta achar pela tag
        if (player == null)
        {
            GameObject found = GameObject.FindGameObjectWithTag("Player");
            if (found != null)
                player = found.transform;
        }
    }

    public virtual void TakeDamage(float amount)
    {
        if (isDead) return;
        currentHealth -= amount;
        if (currentHealth <= 0f) Die();
    }

    protected virtual void Die()
    {
        isDead = true;
        Destroy(gameObject);
    }

    protected float DistanceToPlayer()
    {
        if (player == null) return Mathf.Infinity;
        return Vector2.Distance(transform.position, player.position);
    }

    protected virtual void OnDrawGizmosSelected()
    {
        // gizmo amarelo pra ver o alcance de detecção no editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
