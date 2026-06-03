using UnityEngine;

// Habilidade: Depulso — repulsa o inimigo com knockback para longe do player.
// Criar: Assets > Create > QuimioBlast > Habilidades > Depulso
// Input:  Tecla 4

[CreateAssetMenu(fileName = "DepulsoAbility", menuName = "QuimioBlast/Habilidades/Depulso")]
public class DepulsoAbility : AbilityBase
{
    [Header("Repulsão")]
    [Tooltip("Intensidade do knockback.")]
    public float forcaRepulsao = 25f;

    [Tooltip("Tempo em segundos que o inimigo fica atordoado enquanto voa para longe.")]
    public float tempoAtordoamento = 0.5f;

    public override void Executar(CombatManager owner, Transform alvo)
    {
        EnemyBase inimigo = alvo.GetComponent<EnemyBase>();
        if (inimigo != null)
            inimigo.TakeDamage(dano);

        // Atordoa primeiro para que o FixedUpdate do inimigo não cancele o knockback.
        if (inimigo != null)
            inimigo.Atordoar(tempoAtordoamento);

        Rigidbody2D rbInimigo = alvo.GetComponent<Rigidbody2D>();
        if (rbInimigo != null)
        {
            Vector2 direcao = ((Vector2)alvo.position - (Vector2)owner.transform.position).normalized;
            rbInimigo.linearVelocity = direcao * forcaRepulsao;
        }

        if (prefabVFX != null)
            Object.Instantiate(prefabVFX, alvo.position, Quaternion.identity);
    }
}
