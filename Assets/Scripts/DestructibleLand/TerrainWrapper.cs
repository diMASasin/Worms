using System;
using System.Collections.Generic;
using Battle_;
using Configs;
using ScriptBoy.Digable2DTerrain.Scripts;
using Spawn;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace DestructibleLand
{
    public class TerrainWrapper : MonoBehaviour
    {
        [SerializeField] private Terrain2D _terrain;
        [SerializeField] private MapBounds _mapBounds;
        [SerializeField] private PolygonCollider2D _polygonCollider;
        
        private Vector2[] _points;
        private List<Edge> _edges;
        private float _maxSlope;
        private ContactFilter2D _contactFilter2D;

        [Inject]
        public void Construct(WormsSpawnerConfig spawnerConfig)
        {
            _contactFilter2D = spawnerConfig.ContactFilter;
            _maxSlope = spawnerConfig.MaxSlope;
        }

        public void GetEdgesForSpawn()
        {
            _edges = new List<Edge>();
            var points = _polygonCollider.points;
            int length = points.Length;
            _points = new Vector2[length];

            Array.Copy(points, _points, length);

            for (int i = 0, j = 0; i < _points.Length; i++)
            {
                j = j >= _points.Length - 1 ? 0 : i + 1;

                var newEdge = new Edge(_points[i], _points[j]);

                if (newEdge.IsFloor(_terrain) &&
                    newEdge.IsSuitableSlope(_maxSlope) &&
                    newEdge.InBounds(_mapBounds, _terrain))
                    _edges.Add(newEdge);
            }
        }

        public Vector2 GetRandomSpawnPoint(Vector2 colliderSize)
        {
            Vector2 randomPoint = Vector2.zero;
            const int tries = 100;

            for (int i = 0; i < tries && i < _edges.Count; i++)
            {
                int random = Random.Range(0, _edges.Count);
                var randomEdge = _edges[random];
                randomPoint = Vector2.Lerp(randomEdge.Point1, randomEdge.Point2, Random.value);
                randomPoint += (Vector2)_terrain.transform.position;
                randomPoint.y += colliderSize.y / 2;

                if (CanFitWormInPosition(randomPoint, colliderSize) == true)
                    break;
                
                if (i >= tries - 1) 
                    Debug.LogWarning($"Worm didn't fit for {tries} tries");
            }
            
            return randomPoint;
        }

        private bool CanFitWormInPosition(Vector2 randomPoint, Vector2 size)
        {
            List<Collider2D> results = new();
            size.x *= 2;

            var overlap =
                Physics2D.OverlapCapsule(randomPoint, size,
                    CapsuleDirection2D.Vertical, 0, _contactFilter2D, results);

            return overlap == 0;
        }
    }
}