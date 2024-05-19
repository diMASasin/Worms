using UnityEditor;
using UnityEngine;

namespace DestructibleLand
{
    [ExecuteAlways]
    public class ColliderRenderer : MonoBehaviour
    {
        [SerializeField] PolygonCollider2D _collider;
        [SerializeField] MeshFilter _meshFilter;
        [SerializeField] bool _showPointsIndex;

        private void Update()
        {
            if (transform.hasChanged)
            {
                CreateMesh();
            }
        }
        private void OnValidate()
        {
            CreateMesh();
        }
        public void CreateMesh()
        {
            if (_meshFilter == null)
                return;

            Mesh mesh = _collider.CreateMesh(true, true);
            _meshFilter.mesh = mesh;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (_showPointsIndex)
                ShowPointsIndex();
        }


        private void ShowPointsIndex()
        {
            for (int p = 0; p < _collider.pathCount; p++)
            {
                for (int i = 0; i < _collider.GetPath(p).Length; i++)
                {
                    Handles.Label(_collider.transform.TransformPoint(_collider.GetPath(p)[i]), i.ToString());
                }
            }
        }
#endif
    }
}
