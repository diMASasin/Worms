using UnityEngine;

public class Edge
{
    private Vector2 _point1;
    private Vector2 _point2;

    public Vector2 Point1 => _point1;
    public Vector2 Point2 => _point2;

    public Edge(Vector2 point1, Vector2 point2)
    {
        _point1 = point1;
        _point2 = point2;
    }

    public bool IsFloor(Land land)
    {
        return !land.PolygonCollider2D.OverlapPoint(new Vector2(_point1.x, _point1.y + 0.1f)) &&
            !land.PolygonCollider2D.OverlapPoint(new Vector2(_point2.x, _point2.y + 0.1f));
    }

    public bool IsSuitableSlope(float maxDegrees)
    {
        float kx, ky;
        kx = _point2.y - _point1.y;
        ky = _point2.x - _point1.x;
        kx /= ky;

        return Mathf.Abs(kx) < Mathf.Tan(maxDegrees);
    }
}
