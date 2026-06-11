using UnityEngine;

// Câmera que segue o player mantendo-o sempre centralizada.
// Adicione este componente na Main Camera e arraste o Player para o campo "Alvo".

[AddComponentMenu("QuimioBlast/Camera Follow")]
public class CameraFollow : MonoBehaviour
{
    [Tooltip("Transform do player que a câmera deve seguir.")]
    public Transform alvo;

    [Tooltip("Velocidade de suavização. Valores maiores = câmera mais responsiva.")]
    [Range(1f, 20f)]
    public float suavizacao = 8f;

    private void LateUpdate()
    {
        if (alvo == null) return;

        Vector3 destino = new Vector3(alvo.position.x, alvo.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, destino, suavizacao * Time.deltaTime);
    }
}
