using UnityEngine;

public class DummyEnemy : EnemyBase
{
    protected override void Die()
    {
        Debug.Log($"[Teste] {gameObject.name} morreu!");
        base.Die();
    }

    public override void TakeDamage(float amount)
    {
        Debug.Log($"[Teste] {gameObject.name} recebeu {amount} de dano. HP: {currentHealth - amount}/{maxHealth}");
        base.TakeDamage(amount);
    }
}
