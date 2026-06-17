using UnityEngine;

[CreateAssetMenu(fileName = "IncendioAbility", menuName = "QuimioBlast/Habilidades/Incendio")]
public class IncendioAbility : AbilityBase
{
    [Header("Projétil")]
    public float velocidadeProjetil = 14f;

    public override bool RequireAlvo => false;

    public override bool PodeExecutar(CombatManager owner, Transform alvo)
    {
        if (alvo == null) return false;
        return Vector2.Distance(owner.transform.position, alvo.position) <= alcance;
    }

    public override void Executar(CombatManager owner, Transform alvo)
    {
        if (prefabProjetil == null)
        {
            Debug.LogWarning("[Confringo] Prefab do projétil não configurado!");
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

        GameObject obj = Object.Instantiate(prefabProjetil, owner.transform.position, Quaternion.identity);
        Projectile2D proj = obj.GetComponent<Projectile2D>();
        if (proj != null)
            proj.Inicializar(direcao, dano, velocidadeProjetil, alcance, owner.GetComponent<Collider2D>());
        else
            Debug.LogWarning("[Confringo] Prefab não tem componente Projectile2D!");

        if (prefabVFX != null)
            Object.Instantiate(prefabVFX, owner.transform.position, Quaternion.identity);
    }
}
