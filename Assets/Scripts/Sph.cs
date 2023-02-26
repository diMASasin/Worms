using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public struct IntersectionPoint
{
    public int LandIndex;
    public int SphIndex;
    public Vector2 Position;
    public bool CCW;

    public IntersectionPoint(int landIndex, int sphIndex, Vector2 position, bool ccw)
    {
        LandIndex = landIndex;
        SphIndex = sphIndex;
        Position = position;
        CCW = ccw;
    }
}


public class Sph : MonoBehaviour
{


    [SerializeField] MeshFilter _meshFilter;
    [SerializeField] int _sides;

    [SerializeField] PolygonCollider2D _sphCollider;
    [SerializeField] PolygonCollider2D _landCollider;

    [SerializeField] List<IntersectionPoint> IntersectionPointsList = new List<IntersectionPoint>();

    private void Start()
    {
        CreateMesh();
    }

    private void OnValidate()
    {
        CreateMesh();
    }

    private void Update()
    {
        CreateMesh();
        if (Input.GetMouseButtonDown(1)) {
            FindIntersections();
        }
    }

    [ContextMenu("CreateMesh")]
    void CreateMesh()
    {
        _sphCollider.CreatePrimitive(_sides, Vector2.one);
        Mesh mesh = _sphCollider.CreateMesh(true, true); // false, false
        _meshFilter.mesh = mesh;
    }

    [ContextMenu("FindIntersections")]
    public void FindIntersections()
    {
        IntersectionPointsList.Clear();

        Vector2[] landPathWorld = _landCollider.GetPath(0);
        for (int i = 0; i < landPathWorld.Length; i++)
            landPathWorld[i] = _landCollider.transform.TransformPoint(landPathWorld[i]);

        Vector2[] sphPathWorld = _sphCollider.GetPath(0);
        for (int i = 0; i < sphPathWorld.Length; i++)
            sphPathWorld[i] = _sphCollider.transform.TransformPoint(sphPathWorld[i]);

        for (int l = 0; l < landPathWorld.Length; l++)
        {
            int landIndex = l;
            int nextLandIndex = l + 1;
            if (nextLandIndex > landPathWorld.Length - 1)
            {
                nextLandIndex = 0;
            }
            Vector2 a = landPathWorld[landIndex];
            Vector2 b = landPathWorld[nextLandIndex];

            for (int s = 0; s < sphPathWorld.Length; s++)
            {
                int sphIndex = s;
                int nextSphIndex = s + 1;
                if (nextSphIndex > sphPathWorld.Length - 1)
                {
                    nextSphIndex = 0;
                }
                Vector2 c = sphPathWorld[sphIndex];
                Vector2 d = sphPathWorld[nextSphIndex];

                if (Intersection.IsIntersecting(a, b, c, d))
                {
                    Vector2 intersection = Intersection.GetIntersection(a, b, c, d);
                    Debug.DrawRay(intersection, Vector3.up * 0.2f, Color.red, 5f);
                    bool ccw = _landCollider.ClosestPoint(c) == c;
                    IntersectionPoint intersectionPoint = new IntersectionPoint(landIndex, sphIndex, intersection, !ccw);
                    IntersectionPointsList.Add(intersectionPoint);
                }
            }
        }

        List<Vector2> newPath = landPathWorld.ToList();
        Debug.Log(newPath.Count);
        for (int i = 0; i < IntersectionPointsList.Count; i += 2)
        {
            int sphereStart = IntersectionPointsList[i].SphIndex;
            int sphereEnd = IntersectionPointsList[i + 1].SphIndex;
            int landStart = IntersectionPointsList[i].LandIndex;
            int landEnd = IntersectionPointsList[i + 1].LandIndex;

            int landIndex = landStart;
            while (true)
            {
                Debug.Log(landIndex + " " + landStart + " " + landEnd);
                if (landIndex == landEnd) break;
                landIndex = GetNext(landIndex, _landCollider.GetTotalPointCount(), true);
                newPath[landIndex] = Vector2.zero;
            }

            int sphIndex = sphereStart;
            List<Vector2> ark = new List<Vector2>();
            while (true)
            {
                if (sphIndex == sphereEnd) break;
                Vector2 pos = sphPathWorld[sphIndex];
                ark.Add(pos);
                // проходимся по кругу по часовой стрелке
                sphIndex = GetNext(sphIndex, _sphCollider.GetTotalPointCount(), false);//IntersectionPointsList[i].CW
                
                
            }
            newPath.InsertRange(landStart + 1, ark);

            for (int d = 0; d < newPath.Count; d++)
            {
                if (newPath[d] == Vector2.zero) {
                    newPath.RemoveAt(d);
                }
            }

        }
        //_landCollider.GetComponent<Land>().SetPath(newPath);
    }

    int GetNext(int index, int length, bool isCW) {
        int nextIndex = index + (isCW ? 1 : -1);
        if (nextIndex >= length) {
            nextIndex = 0;
        } else if (nextIndex < 0) {
            nextIndex = length - 1;
        }
        return nextIndex;
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < _sphCollider.GetTotalPointCount(); i++)
        {
            Handles.Label(_sphCollider.transform.TransformPoint(_sphCollider.GetPath(0)[i]), i.ToString());
        }
        for (int i = 0; i < _landCollider.GetTotalPointCount(); i++)
        {
            Handles.Label(_landCollider.transform.TransformPoint(_landCollider.GetPath(0)[i]), i.ToString());
        }

    }

}
