using UnityEngine;

// Habilidade: Accio — puxa o inimigo mais próximo em direção ao player e o atordoa.
// Criar: Assets > Create > QuimioBlast > Habilidades > Accio
// Input:  Tecla 3

[CreateAssetMenu(fileName = "AccioAbility", menuName = "QuimioBlast/Habilidades/Accio")]
public class AccioAbility : AbilityBase
{
    [Header("Puxão")]
    [Tooltip("Intensidade da força de puxão (ForceMode2D.Impulse).")]
    public float forcaPuxao = 20f;

    [Tooltip("Tempo em segundos que o inimigo fica atordoado após ser puxado.")]
    public float tempoAtordoamento = 2f;

    public override void Executar(CombatManager owner, Transform alvo)
    {
        EnemyBase inimigo = alvo.GetComponent<EnemyBase>();
        if (inimigo != null)
            inimigo.TakeDamage(dano);

        // Atordoa primeiro para que o FixedUpdate do inimigo pare de sobrescrever
        // a velocidade antes de aplicarmos o puxão.
        if (inimigo != null)
            inimigo.Atordoar(tempoAtordoamento);

        Rigidbody2D rbInimigo = alvo.GetComponent<Rigidbody2D>();
        if (rbInimigo != null)
        {
            Vector2 direcao = ((Vector2)owner.transform.position - (Vector2)alvo.position).normalized;
            rbInimigo.linearVelocity = direcao * forcaPuxao;
        }

        if (prefabVFX != null)
            Object.Instantiate(prefabVFX, alvo.position, Quaternion.identity);
    }
}
