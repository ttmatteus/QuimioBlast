using UnityEngine;

[AddComponentMenu("QuimioBlast/Camera Follow")]
public class CameraFollow : MonoBehaviour
{
    [Tooltip("Transform do player que a câmera deve seguir.")]
    public Transform alvo;

    [Tooltip("Velocidade de suavização. Valores maiores = câmera mais responsiva.")]
    [Range(1f, 20f)]
    public float suavizacao = 8f;

    [Header("Limites do Mapa")]
    [Tooltip("Ativa o clamp da câmera dentro dos limites definidos abaixo.")]
    public bool usarLimites = false;

    [Tooltip("Limite mínimo X e Y (canto inferior-esquerdo do mapa).")]
    public Vector2 limiteMin;

    [Tooltip("Limite máximo X e Y (canto superior-direito do mapa).")]
    public Vector2 limiteMax;

    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        if (alvo == null) return;

        Vector3 destino = new Vector3(alvo.position.x, alvo.position.y, transform.position.z);
        Vector3 novaPos = Vector3.Lerp(transform.position, destino, suavizacao * Time.deltaTime);

        if (usarLimites && cam != null)
        {
            float alturaMetade = cam.orthographicSize;
            float larguraMetade = alturaMetade * cam.aspect;

            novaPos.x = Mathf.Clamp(novaPos.x, limiteMin.x + larguraMetade, limiteMax.x - larguraMetade);
            novaPos.y = Mathf.Clamp(novaPos.y, limiteMin.y + alturaMetade, limiteMax.y - alturaMetade);
        }

        transform.position = novaPos;
    }

    private void OnDrawGizmosSelected()
    {
        if (!usarLimites) return;
        Gizmos.color = Color.cyan;
        Vector3 centro = new Vector3((limiteMin.x + limiteMax.x) / 2f, (limiteMin.y + limiteMax.y) / 2f, 0f);
        Vector3 tamanho = new Vector3(limiteMax.x - limiteMin.x, limiteMax.y - limiteMin.y, 0f);
        Gizmos.DrawWireCube(centro, tamanho);
    }
}
