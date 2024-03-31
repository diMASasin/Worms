using UnityEngine;

public class Test : MonoBehaviour
{

    [SerializeField] Transform A,B,C,D;
    [SerializeField] Transform W;

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(A.position, B.position);
        Gizmos.DrawLine(C.position, D.position);
        W.position = Intersection.GetIntersection(A.position, B.position, C.position, D.position);
    }

}
