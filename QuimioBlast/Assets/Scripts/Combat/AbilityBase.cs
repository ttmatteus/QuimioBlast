using UnityEngine;

// Classe-base para todas as habilidades do jogo.
// Para criar uma nova habilidade:
//   1. Crie um script que herde de AbilityBase (ex: MinhaHabilidade : AbilityBase).
//   2. Implemente o método Executar().
//   3. No Unity: Assets > Create > QuimioBlast > Habilidades > [nome da habilidade].
//   4. Arraste o asset criado para o slot correspondente no CombatManager.

public abstract class AbilityBase : ScriptableObject
{
    [Header("Identificação")]
    public string nomeHabilidade = "Nova Habilidade";

    [Header("Estatísticas")]
    public float dano     = 20f;
    public float cooldown = 1f;
    public float alcance  = 5f;

    [Header("Visual")]
    [Tooltip("Partículas ou VFX instantâneo da habilidade.")]
    public GameObject prefabVFX;

    [Tooltip("Prefab do projétil (preencha apenas em habilidades de projétil).")]
    public GameObject prefabProjetil;

    [Header("Animação")]
    [Tooltip("Nome do parâmetro Trigger no Animator do Player (deixe vazio se não tiver).")]
    public string parametroAnimacao = "";

    // Quando false, a habilidade pode disparar mesmo sem inimigo no alcance.
    public virtual bool RequireAlvo => true;

    // Verificação extra chamada antes de animar e consumir cooldown.
    // Sobrescreva para bloquear a execução por condições próprias (ex: alcance curto).
    public virtual bool PodeExecutar(CombatManager owner, Transform alvo) => true;

    // Executa a lógica da habilidade.
    // owner  = o CombatManager (MonoBehaviour) que dispara a habilidade.
    // alvo   = Transform do inimigo selecionado (pode ser null se RequireAlvo == false).
    public abstract void Executar(CombatManager owner, Transform alvo);
}
