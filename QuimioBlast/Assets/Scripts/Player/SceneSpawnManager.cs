using UnityEngine;

public class SceneSpawnManager : MonoBehaviour
{
    [Header("Configuração de Spawn")]
    [SerializeField] private Transform pontoDeOrigem; // Arraste o objeto "PontoSpawn_Inicial" aqui

    private void Start()
    {
        ReposicionarPlayer();
    }

    private void ReposicionarPlayer()
    {
        if (pontoDeOrigem == null) return;

        GameObject player = GameObject.FindWithTag("Player");

        if (player != null)
        {
            // DICA SÊNIOR: Se o seu Player usa Rigidbody2D, mudar o transform.position diretamente 
            // pode causar conflito com o motor de física ou dar "glitch" visual na interpolação.
            // O correto é teleportar o Rigidbody2D.
            if (player.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
            {
                rb.position = pontoDeOrigem.position;
            }
            else
            {
                player.transform.position = pontoDeOrigem.position;
            }

            // Garante que a Unity atualize a física no mesmo frame para evitar puxões
            Physics2D.SyncTransforms();

            Debug.Log("[SpawnManager] Player reposicionado com sucesso na nova cena.");
        }
    }
}