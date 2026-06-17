using UnityEngine;
using System.Collections;

// SpawnManager da fase de Boss.
// Spawna TODOS os inimigos cadastrados de uma só vez a cada <intervaloSpawn> segundos.
// Diferente do SpawnManager normal, NÃO espera os inimigos morrerem — usa tempo fixo.
//
// Como configurar:
//   1. Crie um GameObject vazio na cena (ex: "SpawnManagerBoss").
//   2. Adicione este componente.
//   3. Em "Entradas Spawn", defina ponto de spawn + prefab para cada inimigo.
//   4. Ajuste "Intervalo Spawn" (padrão: 20 s).

public class SpawnManagerBoss : MonoBehaviour
{
    [System.Serializable]
    public struct EntradaSpawn
    {
        [Tooltip("Transform que define posição de spawn.")]
        public Transform ponto;
        [Tooltip("Prefab do inimigo a ser instanciado.")]
        public GameObject prefab;
    }

    [Header("Inimigos")]
    public EntradaSpawn[] entradasSpawn;

    [Header("Timing")]
    [Tooltip("Intervalo em segundos entre cada leva de spawn.")]
    public float intervaloSpawn = 20f;

    private void Start()
    {
        SpawnarLeva();
        StartCoroutine(LoopSpawn());
    }

    private IEnumerator LoopSpawn()
    {
        while (true)
        {
            yield return new WaitForSeconds(intervaloSpawn);
            SpawnarLeva();
        }
    }

    private void SpawnarLeva()
    {
        foreach (var entrada in entradasSpawn)
        {
            if (entrada.ponto == null || entrada.prefab == null) continue;
            Instantiate(entrada.prefab, entrada.ponto.position, entrada.ponto.rotation);
        }
    }
}
