using System;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class WormsSpawner : MonoBehaviour
{
    [SerializeField] private int _teamsNumber = 2;
    [SerializeField] private int _wormsNumber = 4;
    [SerializeField] private List<Color> _teamColors;

    [SerializeField] private Land _land;
    [SerializeField] private Team _teamTemplate;
    [SerializeField] private Worm _wormTemplate;
    [SerializeField] private Transform _container;
    [SerializeField] private float _maxSlope = 45;

    private Vector2[] _points;
    private List<Edge> _edges = new();

    public event UnityAction<Worm> WormSpawned;

    public List<Team> SpawnTeams()
    {
        if(_teamColors.Count < _teamsNumber)
            throw new ArgumentOutOfRangeException($"{nameof(_teamColors)} count cant be less then {nameof(_teamsNumber)}");

        List<Team> teams = new List<Team>();

        for (int i = 0; i < _teamsNumber; i++)
        {
            var randomColor = _teamColors[Random.Range(0, _teamColors.Count)];
            _teamColors.Remove(randomColor);

            var newTeam = SpawnTeam(_wormsNumber, randomColor, $"Team {i + 1}");
            teams.Add(newTeam);
        }
        return teams;
    }

    private Team SpawnTeam(int wormsNumber, Color teamColor, string name)
    {
        List<Worm> worms = new List<Worm>();

        var team = Instantiate(_teamTemplate, _container);

        for (int i = 0; i < wormsNumber; i++)
        {
            var newWorm = Instantiate(_wormTemplate, GetRandomPointForSpawn(), Quaternion.identity, team.transform);
            newWorm.SetRigidbodyKinematic();
            WormSpawned?.Invoke(newWorm);
            worms.Add(newWorm);
        }

        team.Init(worms, teamColor, name);
        team.name = name;
        return team;
    }

    public void GetEdgesForSpawn()
    {
        int length = _land.PolygonCollider2D.points.Length;
        _points = new Vector2[length];
        Array.Copy(_land.PolygonCollider2D.points, _points, length);

        for (int i = 0; i < _points.Length; i++)
        {
            int j = i + 1;
            if (j >= _points.Length)
                j = 0;

            var newEdge = new Edge(_points[i], _points[j]);
            if (newEdge.IsFloor(_land) && newEdge.IsSuitableSlope(_maxSlope))
                _edges.Add(newEdge);
        }
    }

    private Vector2 GetRandomPointForSpawn()
    {
        Vector2 randomPoint;
        do
        {
            int random = Random.Range(0, _edges.Count);
            var randomEdge = _edges[random];
            randomPoint = Vector2.Lerp(randomEdge.Point1, randomEdge.Point2, Random.value);
            randomPoint += (Vector2)_land.transform.position;
        }
        while (!CanFitWormInPosition(randomPoint));

        return randomPoint;
    }

    private bool CanFitWormInPosition(Vector2 position)
    {
        return !Physics2D.OverlapCapsule(position + new Vector2(0, 0.5f), _wormTemplate.Collider2D.size, CapsuleDirection2D.Vertical, 0);
    }
}
