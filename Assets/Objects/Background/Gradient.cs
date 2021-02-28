using System.Linq;
using UnityEngine;

public class Gradient : MonoBehaviour
{
    private MeshFilter _meshFilter;

    [SerializeField]
    private Color _bottomColor;
    [SerializeField]
    private Color _topColor;

    private void Awake()
    {
        _meshFilter = GetComponent<MeshFilter>();
        Debug.Assert(_meshFilter != null);
    }

    void Start()
    {
        var mesh = _meshFilter.mesh;
        var vertices = mesh.vertices;
        var minY = vertices.Select(vertex => vertex.y).Min();
        var maxY = vertices.Select(vertex => vertex.y).Max();
        Color[] colors = new Color[vertices.Length];
        for (int i = 0; i < vertices.Length; ++i)
        {
            float relativeY = (vertices[i].y - minY) / maxY;
            colors[i] = Color.Lerp(_bottomColor, _topColor, relativeY);
        }
        mesh.colors = colors;
    }
}
