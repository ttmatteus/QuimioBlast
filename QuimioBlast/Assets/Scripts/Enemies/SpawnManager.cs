using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Como configurar no Inspector:
//   1. Crie um GameObject vazio na cena (ex: "SpawnManager").
//   2. Adicione este componente nele.
//   3. Em "Entradas Spawn", defina quantas entradas quiser.
//      Cada entrada tem: Ponto (Transform de spawn) + Prefab (inimigo pré-configurado).
//   4. Crie GameObjects vazios na cena para servir como pontos de spawn e arraste-os.

public class SpawnManager : MonoBehaviour
{
    [System.Serializable]
    public struct EntradaSpawn
    {
        [Tooltip("Transform que define posição e rotação do spawn.")]
        public Transform ponto;
        [Tooltip("Prefab do inimigo a ser instanciado nesse ponto.")]
        public GameObject prefab;
    }

    [Header("Inimigos")]
    public EntradaSpawn[] entradasSpawn;

    [Header("Timing")]
    [Tooltip("Segundos de espera após a morte do último inimigo antes de spawnar nova leva.")]
    public float delayEntreLevas = 3f;

    private readonly List<EnemyBase> inimigosAtivos = new();
    private bool aguardandoSpawn;

    private void Start() => SpawnarLeva();

    private void Update()
    {
        if (aguardandoSpawn || inimigosAtivos.Count == 0) return;

        foreach (var inimigo in inimigosAtivos)
            if (inimigo != null && !inimigo.IsDead) return;

        aguardandoSpawn = true;
        StartCoroutine(DelayESpawnar());
    }

    private IEnumerator DelayESpawnar()
    {
        yield return new WaitForSeconds(delayEntreLevas);
        SpawnarLeva();
        aguardandoSpawn = false;
    }

    private void SpawnarLeva()
    {
        inimigosAtivos.Clear();

        foreach (var entrada in entradasSpawn)
        {
            if (entrada.ponto == null || entrada.prefab == null) continue;

            GameObject go = Instantiate(entrada.prefab, entrada.ponto.position, entrada.ponto.rotation);
            EnemyBase inimigo = go.GetComponent<EnemyBase>();
            if (inimigo != null)
                inimigosAtivos.Add(inimigo);
        }
    }
}
