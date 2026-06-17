using UnityEngine;

// Habilidade: Diffindo — corte de longo alcance, dano alto em alvo único.
// Criar: Assets > Create > QuimioBlast > Habilidades > Diffindo
// Input:  Tecla 2
// Requer: campo "Prefab Projetil" preenchido (use um projétil mais rápido/cortante).

[CreateAssetMenu(fileName = "DiffindoAbility", menuName = "QuimioBlast/Habilidades/Diffindo")]
public class DiffindoAbility : AbilityBase
{
    [Header("Projétil")]
    public float velocidadeProjetil = 18f;

    public override bool RequireAlvo => false;

    public override void Executar(CombatManager owner, Transform alvo)
    {
        if (prefabProjetil == null)
        {
            Debug.LogWarning("[Diffindo] Prefab do projétil não configurado no ScriptableObject!");
            return;
        }

        Vector2 direcao;
        if (alvo != null)
            direcao = ((Vector2)alvo.position - (Vector2)owner.transform.position).normalized;
        else
        {
            Camera cam = Camera.main;
            if (cam == null) return;
            Vector2 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
            direcao = (mouseWorld - (Vector2)owner.transform.position).normalized;
            if (direcao.magnitude < 0.01f) direcao = Vector2.right;
        }

        GameObject obj  = Object.Instantiate(prefabProjetil, owner.transform.position, Quaternion.identity);
        Projectile2D proj = obj.GetComponent<Projectile2D>();
        if (proj != null)
            proj.Inicializar(direcao, dano, velocidadeProjetil, alcance, owner.GetComponent<Collider2D>());
        else
            Debug.LogWarning("[Diffindo] O prefab do projétil não tem o componente Projectile2D!");

        if (prefabVFX != null)
            Object.Instantiate(prefabVFX, owner.transform.position, Quaternion.identity);
    }
}
