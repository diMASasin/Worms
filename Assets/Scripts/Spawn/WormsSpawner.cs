using ScriptBoy.Digable2DTerrain;
using System;
using System.Collections.Generic;
using Configs;
using Factories;
using UnityEngine;
using Random = UnityEngine.Random;

public class WormsSpawner : MonoBehaviour
{
    [SerializeField] private WormsSpawnerConfig _spawnerConfig;
    [SerializeField] private Terrain2D _terrain;
    [SerializeField] private Worm _wormTemplate;
    [SerializeField] private MapBounds _mapBounds;

    private TeamFactory _teamFactory;
    private Vector2[] _points;
    private readonly List<Edge> _edges = new();

    private List<Color> TeamColors => _spawnerConfig.TeamColors;

    public void Init(TeamFactory teamFactory)
    {
        _teamFactory = teamFactory;
        
        GetEdgesForSpawn();
    }

    public void Spawn(out List<Worm> worms, out List<Team> teams)
    {
        worms = new List<Worm>();
        teams = new List<Team>();
        
        foreach (var teamConfig in _spawnerConfig.TeamConfigs)
        {
            Color randomColor = TeamColors[Random.Range(0, TeamColors.Count)];
            Team team = _teamFactory.Create(randomColor, transform, teamConfig);

            teams.Add(team);
            worms.AddRange(team.Worms);
        }
        
        MoveToSpawnPoint(worms, GetRandomSpawnPoint);
    }

    private void MoveToSpawnPoint(List<Worm> worms, Func<Vector2> getSpawnPoint)
    {
        foreach (var worm in worms)
            worm.transform.position = getSpawnPoint();
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

    private Vector2 GetRandomSpawnPoint()
    {
        Vector2 randomPoint;
        do
        {
            int random = Random.Range(0, _edges.Count);
            var randomEdge = _edges[random];
            randomPoint = Vector2.Lerp(randomEdge.Point1, randomEdge.Point2, Random.value);
            randomPoint += (Vector2)_terrain.transform.position;
        }
        while (!CanFitWormInPosition(randomPoint));

        return randomPoint;
    }

    private bool CanFitWormInPosition(Vector2 position)
    {
        var colliderSize = _wormTemplate.Collider2D.size;
        var size = new Vector2(colliderSize.x * 2, colliderSize.y);
        
        return !Physics2D.OverlapCapsule(position + new Vector2(0, 0.5f), size, CapsuleDirection2D.Vertical, 0);
    }
}