using UnityEngine;

// Roda depois do CinemachineBrain e trava a câmera dentro dos limites definidos.
// Adicione este script na Main Camera da cena.
[DefaultExecutionOrder(1000)]
[AddComponentMenu("QuimioBlast/Camera Clamp")]
public class CameraClamp : MonoBehaviour
{
    [Header("Limites do Mapa")]
    public Vector2 limiteMin;
    public Vector2 limiteMax;

    private Camera cam;

    private void Awake() => cam = GetComponent<Camera>();

    private void LateUpdate()
    {
        if (cam == null) return;

        float halfH = cam.orthographicSize;
        float halfW = halfH * cam.aspect;

        var pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, limiteMin.x + halfW, limiteMax.x - halfW);
        pos.y = Mathf.Clamp(pos.y, limiteMin.y + halfH, limiteMax.y - halfH);
        transform.position = pos;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        var centro = new Vector3((limiteMin.x + limiteMax.x) / 2f, (limiteMin.y + limiteMax.y) / 2f, 0f);
        var tamanho = new Vector3(limiteMax.x - limiteMin.x, limiteMax.y - limiteMin.y, 0f);
        Gizmos.DrawWireCube(centro, tamanho);
    }
}
