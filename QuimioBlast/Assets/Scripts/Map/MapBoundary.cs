using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class MapBoundary : MonoBehaviour
{
    [System.Serializable]
    public struct ZonaColisao
    {
        public string nome;
        public Vector2 centro;
        public Vector2 tamanho;
    }

    [Header("Uma entrada por área sólida — ajuste no Inspector")]
    public List<ZonaColisao> zonas = new List<ZonaColisao>();

    private void Awake()
    {
        foreach (var zona in zonas)
        {
            var go = new GameObject("Colis_" + (string.IsNullOrEmpty(zona.nome) ? "Zona" : zona.nome));
            go.transform.SetParent(transform);
            go.transform.position = new Vector3(zona.centro.x, zona.centro.y, 0f);
            go.AddComponent<BoxCollider2D>().size = zona.tamanho;
        }
    }

    private void Start()
    {
        if (AstarPath.active != null)
            AstarPath.active.Scan();
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (zonas == null) return;
        Gizmos.color = new Color(1f, 0.3f, 0.3f, 0.2f);
        foreach (var zona in zonas)
            Gizmos.DrawCube(new Vector3(zona.centro.x, zona.centro.y), new Vector3(zona.tamanho.x, zona.tamanho.y));
    }

    private void OnDrawGizmosSelected()
    {
        if (zonas == null) return;
        Gizmos.color = new Color(1f, 0.2f, 0.2f, 0.85f);
        foreach (var zona in zonas)
            Gizmos.DrawWireCube(new Vector3(zona.centro.x, zona.centro.y), new Vector3(zona.tamanho.x, zona.tamanho.y));
    }
#endif
}
