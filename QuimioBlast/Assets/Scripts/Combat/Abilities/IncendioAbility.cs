using UnityEngine;

// Habilidade: Incêndio — explosão de fogo em área (AoE) ao redor do player.
// Criar: Assets > Create > QuimioBlast > Habilidades > Incendio
// Input:  Tecla 1
// Dica:   "raioAoE" define o raio de dano; "alcance" em AbilityBase não é usado aqui.

[CreateAssetMenu(fileName = "IncendioAbility", menuName = "QuimioBlast/Habilidades/Incendio")]
public class IncendioAbility : AbilityBase
{
    [Header("Área de Efeito")]
    [Tooltip("Raio da explosão de fogo ao redor do player.")]
    public float raioAoE = 3f;

    public override void Executar(CombatManager owner, Transform alvo)
    {
        // VFX centralizado no player
        if (prefabVFX != null)
            Object.Instantiate(prefabVFX, owner.transform.position, Quaternion.identity);

        // Dano em todos os inimigos dentro do raio
        Collider2D[] atingidos = Physics2D.OverlapCircleAll(owner.transform.position, raioAoE);
        foreach (var col in atingidos)
        {
            EnemyBase inimigo = col.GetComponentInParent<EnemyBase>();
            if (inimigo != null && !inimigo.IsDead)
                inimigo.TakeDamage(dano);
        }
    }

    private void OnValidate()
    {
        // Garante que o raio AoE nunca fique negativo ao editar no Inspector
        if (raioAoE < 0f) raioAoE = 0f;
    }
}
