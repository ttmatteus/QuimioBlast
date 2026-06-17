using UnityEngine;
using Unity.Cinemachine; // Namespace oficial da Cinemachine na Unity 6

public class CinemachineTargetBinder : MonoBehaviour
{
    private CinemachineCamera _cinemachineCamera;

    private void Awake()
    {
        _cinemachineCamera = GetComponent<CinemachineCamera>();
    }

    private void Start()
    {
        VincularAlvo();
    }

    public void VincularAlvo()
    {
        // Encontra o Player sobrevivente que veio da outra cena
        GameObject player = GameObject.FindWithTag("Player");

        if (player != null && _cinemachineCamera != null)
        {
            _cinemachineCamera.Follow = player.transform;
            Debug.Log("[Cinemachine] Alvo reatribuído com sucesso para o Player ativo.");
        }
        else
        {
            Debug.LogWarning("[Cinemachine] Não foi possível encontrar um GameObject com a tag 'Player' para a câmera seguir.");
        }
    }
}
