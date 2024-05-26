using System;
using Battle_;
using ScriptBoy.Digable2DTerrain.Scripts;
using UnityEngine;

namespace Spawn
{
    [Serializable]
    public class Edge
    {
        [SerializeField] private Vector2 _point1;
        [SerializeField] private Vector2 _point2;

        private float _kx;
        private float _maxKx;

        public Vector2 Point1 => _point1;
        public Vector2 Point2 => _point2;

        public Edge(Vector2 point1, Vector2 point2)
        {
            _point1 = point1;
            _point2 = point2;
        }

        public bool IsFloor(Terrain2D terrain)
        {
            var terrainPosition = terrain.transform.position;
        
            var point1 = _point1 + (Vector2)terrainPosition;
            var point2 = _point2 + (Vector2)terrainPosition;
            return !terrain.polygonCollider.OverlapPoint(new Vector2(point1.x, point1.y + 0.1f)) &&
                   !terrain.polygonCollider.OverlapPoint(new Vector2(point2.x, point2.y + 0.1f)) &&
                   !terrain.polygonCollider.OverlapPoint(new Vector2(point1.x, point1.y + 0.8f)) &&
                   !terrain.polygonCollider.OverlapPoint(new Vector2(point2.x, point2.y + 0.8f));
        }

        public bool IsSuitableSlope(float maxDegrees)
        {
            float ky;
            _kx = _point2.y - _point1.y;
            ky = _point2.x - _point1.x;
            _kx /= ky;

            maxDegrees = Mathf.Deg2Rad * maxDegrees;
            _maxKx = Mathf.Tan(maxDegrees);

            return Mathf.Abs(_kx) < _maxKx;
        }

        public bool InBounds(MapBounds mapBounds, Terrain2D terrain)
        {
            var position = (Vector2)terrain.transform.position;

            var point1 = _point1 + position;
            var point2 = _point2 + position;

            return IsPointInBounds(point1, mapBounds) && IsPointInBounds(point2, mapBounds);
        }

        private bool IsPointInBounds(Vector2 point, MapBounds bounds)
        {
            return point.x > bounds.Left.position.x && 
                   point.x < bounds.Right.position.x && 
                   point.y < bounds.Top.position.y &&
                   point.y > bounds.Bottom.position.y;
        }
    }
}
