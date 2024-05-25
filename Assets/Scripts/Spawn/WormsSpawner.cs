using System;
using System.Collections.Generic;
using Battle_;
using Configs;
using Factories;
using ScriptBoy.Digable2DTerrain.Scripts;
using UnityEngine;
using WormComponents;
using Random = UnityEngine.Random;

namespace Spawn
{
    public class WormsSpawner : MonoBehaviour
    {
        [SerializeField] private WormsSpawnerConfig _spawnerConfig;
        [SerializeField] private Terrain2D _terrain;
        [SerializeField] private Worm _wormTemplate;
        [SerializeField] private MapBounds _mapBounds;
        [SerializeField] private ContactFilter2D _contactFilter2D;
        
        private TeamFactory _teamFactory;
        private Vector2[] _points;
        private readonly List<Edge> _edges = new();
        private readonly List<Color> _unusedTeamColors = new();

        public void Init(TeamFactory teamFactory)
        {
            _teamFactory = teamFactory;
            _unusedTeamColors.AddRange(_spawnerConfig.TeamColors);
        
            GetEdgesForSpawn();
        }

        public void Spawn(int teamsNumber, int wormsNumber, out CycledList<IWorm> worms, out CycledList<Team> teams)
        {
            worms = new CycledList<IWorm>();
            teams = new CycledList<Team>();

            for (var i = 0; i < teamsNumber; i++)
            {
                var teamConfig = _spawnerConfig.TeamConfigs[i];
                Color randomColor = _unusedTeamColors[Random.Range(0, _unusedTeamColors.Count)];
                _unusedTeamColors.Remove(randomColor);
                Team team = _teamFactory.Create(randomColor, transform, teamConfig, wormsNumber, GetRandomSpawnPoint);

                teams.Add(team);
                worms.AddRange(team.Worms);
            }
        }

        private void GetEdgesForSpawn()
        {
            var points = _terrain.polygonCollider.points;
            int length = points.Length;
            _points = new Vector2[length];

            Array.Copy(points, _points, length);

            for (int i = 0; i < _points.Length; i++)
            {
                int j = i + 1;
                if (j >= _points.Length)
                    j = 0;

                var newEdge = new Edge(_points[i], _points[j]);
                bool edgeInBounds = newEdge.InBounds(_mapBounds.Left.position, _mapBounds.Top.position, 
                    _mapBounds.Right.position, _mapBounds.Bottom.position, _terrain);

                if (newEdge.IsFloor(_terrain) && newEdge.IsSuitableSlope(_spawnerConfig.MaxSlope) && edgeInBounds)
                    _edges.Add(newEdge);
            }
        }

        private Vector2 GetRandomSpawnPoint(Vector2 colliderSize)
        {
            Vector2 randomPoint;
            int i = 0;
            do
            {
                int random = Random.Range(0, _edges.Count);
                var randomEdge = _edges[random];
                randomPoint = Vector2.Lerp(randomEdge.Point1, randomEdge.Point2, Random.value);
                randomPoint += (Vector2)_terrain.transform.position;
                
                i++;
                int tries = 100;
                if (i >= tries)
                {
                    Debug.LogWarning($"Worm didn't fit for {tries} tries");
                    break;
                }
            }
            while (!CanFitWormInPosition(randomPoint, colliderSize));

            return randomPoint;
        }

        private bool CanFitWormInPosition(Vector2 randomPoint, Vector2 size)
        {
            List<Collider2D> results = new();
            size *= 2;
            
            var overlap =
                Physics2D.OverlapCapsule(randomPoint, size, 
                    CapsuleDirection2D.Vertical, 0, _contactFilter2D, results);
            
            return overlap == 0;
        }
    }
}