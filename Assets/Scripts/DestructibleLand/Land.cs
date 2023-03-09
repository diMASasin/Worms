using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[ExecuteAlways]
public class Land : MonoBehaviour
{
    [SerializeField] PolygonCollider2D _collider;
    [SerializeField] ColliderRenderer _colliderRenderer;

    public PolygonCollider2D PolygonCollider2D => _collider;

    public void SetPath(List<List<Point>> paths) {
        _collider.pathCount = paths.Count;
        for (int i = 0; i < paths.Count; i++)
        {
            List<Vector2> path = new List<Vector2>();
            for (int p = 0; p < paths[i].Count; p++)
            {
                path.Add(paths[i][p].Position);
            }
            _collider.SetPath(i, path);
        }
        if(_colliderRenderer != null )
            _colliderRenderer.CreateMesh();
    }

}
