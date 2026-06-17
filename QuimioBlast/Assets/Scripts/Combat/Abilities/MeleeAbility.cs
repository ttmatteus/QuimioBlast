using UnityEngine;

// Habilidade: Ataque Melee (soco/golpe direto).
// Criar: Assets > Create > QuimioBlast > Habilidades > Melee
// Input:  Botão Direito do Mouse
// Tipo:   Curto alcance — só funciona se o inimigo estiver dentro de "alcance".

[CreateAssetMenu(fileName = "MeleeAbility", menuName = "QuimioBlast/Habilidades/Melee")]
public class MeleeAbility : AbilityBase
{
    public override void Executar(CombatManager owner, Transform alvo)
    {
        float distancia = Vector2.Distance(owner.transform.position, alvo.position);

        if (distancia > alcance)
        {
            Debug.Log($"[Melee] Inimigo a {distancia:F1}u — fora do alcance de {alcance}u.");
            return;
        }

        // Aplica dano
        EnemyBase inimigo = alvo.GetComponent<EnemyBase>();
        if (inimigo != null)
            inimigo.TakeDamage(dano, owner.transform.position);

        // VFX no ponto de impacto
        if (prefabVFX != null)
            Object.Instantiate(prefabVFX, alvo.position, Quaternion.identity);
    }
}
