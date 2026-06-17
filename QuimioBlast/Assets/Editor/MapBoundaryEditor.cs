using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

[CustomEditor(typeof(MapBoundary))]
public class MapBoundaryEditor : Editor
{
    private readonly List<BoxBoundsHandle> handles = new List<BoxBoundsHandle>();

    private void OnEnable() => SincronizarHandles();

    private void SincronizarHandles()
    {
        var mb = (MapBoundary)target;
        while (handles.Count < mb.zonas.Count)
            handles.Add(new BoxBoundsHandle { axes = PrimitiveBoundsHandle.Axes.X | PrimitiveBoundsHandle.Axes.Y });
        while (handles.Count > mb.zonas.Count)
            handles.RemoveAt(handles.Count - 1);
    }

    private void OnSceneGUI()
    {
        var mb = (MapBoundary)target;
        SincronizarHandles();

        for (int i = 0; i < mb.zonas.Count; i++)
        {
            var zona = mb.zonas[i];
            var handle = handles[i];

            handle.handleColor = new Color(1f, 0.35f, 0.35f);
            handle.wireframeColor = new Color(1f, 0.35f, 0.35f, 0.4f);
            handle.center = zona.centro;
            handle.size = zona.tamanho;

            EditorGUI.BeginChangeCheck();

            // Alças de redimensionamento nas bordas
            handle.DrawHandle();

            // Alça de movimento no centro (quadrado amarelo)
            Handles.color = new Color(1f, 0.85f, 0.1f);
            float tam = HandleUtility.GetHandleSize(zona.centro) * 0.12f;
            Vector3 novoCentro = Handles.FreeMoveHandle(
                new Vector3(zona.centro.x, zona.centro.y, 0f),
                tam, Vector3.zero, Handles.RectangleHandleCap);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(mb, "Editar Zona de Colisão");
                var z = mb.zonas[i];
                z.tamanho = handle.size;
                z.centro = ((Vector2)novoCentro != zona.centro)
                    ? (Vector2)novoCentro
                    : (Vector2)handle.center;
                mb.zonas[i] = z;
                EditorUtility.SetDirty(mb);
            }

            // Rótulo com nome e dimensões
            string label = string.IsNullOrEmpty(zona.nome) ? $"Zona {i + 1}" : zona.nome;
            Handles.Label(
                new Vector3(zona.centro.x, zona.centro.y + zona.tamanho.y * 0.5f + 0.4f, 0f),
                label, EditorStyles.miniLabel);
        }
    }
}
