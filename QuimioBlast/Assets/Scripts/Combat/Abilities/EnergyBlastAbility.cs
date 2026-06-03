using UnityEngine;

// Habilidade: Disparo de Energia — projétil de longo alcance, dano baixo.
// Criar: Assets > Create > QuimioBlast > Habilidades > EnergyBlast
// Input:  Tecla E
// Requer: campo "Prefab Projetil" preenchido com um GameObject que tenha Projectile2D.

[CreateAssetMenu(fileName = "EnergyBlastAbility", menuName = "QuimioBlast/Habilidades/EnergyBlast")]
public class EnergyBlastAbility : AbilityBase
{
    [Header("Projétil")]
    public float velocidadeProjetil = 12f;

    public override void Executar(CombatManager owner, Transform alvo)
    {
        if (prefabProjetil == null)
        {
            Debug.LogWarning("[EnergyBlast] Prefab do projétil não configurado no ScriptableObject!");
            return;
        }

        Vector2 direcao = ((Vector2)alvo.position - (Vector2)owner.transform.position).normalized;

        GameObject obj = Object.Instantiate(prefabProjetil, owner.transform.position, Quaternion.identity);
        Projectile2D proj = obj.GetComponent<Projectile2D>();
        if (proj != null)
            proj.Inicializar(direcao, dano, velocidadeProjetil, alcance, owner.GetComponent<Collider2D>());
        else
            Debug.LogWarning("[EnergyBlast] O prefab do projétil não tem o componente Projectile2D!");

        // VFX no player ao disparar
        if (prefabVFX != null)
            Object.Instantiate(prefabVFX, owner.transform.position, Quaternion.identity);
    }
}
