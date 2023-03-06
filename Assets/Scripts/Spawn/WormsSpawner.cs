using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class WormsSpawner : MonoBehaviour
{
    [SerializeField] private Land _land;
    [SerializeField] private Camera _camera;
    [SerializeField] private Team _teamTemplate;
    [SerializeField] private Worm _wormTemplate;
    [SerializeField] private Transform _container;
    [SerializeField] private float _maxSlope = 45;

    private Vector2[] _points;
    private List<Edge> edges = new();

    public event UnityAction<Worm> WormSpawned;

    public List<Team> SpawnTeams(int numberOfTeams, int wormsNumber, List<Color> teamColors)
    {
        if(teamColors.Count < numberOfTeams)
            throw new ArgumentOutOfRangeException($"{nameof(teamColors)} count cant be less then {nameof(numberOfTeams)}");

        List<Team> teams = new List<Team>();

        for (int i = 0; i < numberOfTeams; i++)
        {
            var randomColor = teamColors[Random.Range(0, teamColors.Count)];
            teamColors.Remove(randomColor);

            var newTeam = SpawnTeam(wormsNumber, randomColor);
            newTeam.name = $"Team {i + 1}";
            teams.Add(newTeam);
        }
        return teams;
    }

    private Team SpawnTeam(int wormsNumber, Color teamColor)
    {
        List<Worm> worms = new List<Worm>();

        var team = Instantiate(_teamTemplate, _container);

        for (int i = 0; i < wormsNumber; i++)
        {
            var newWorm = Instantiate(_wormTemplate, GetRandomPointForSpawn(), Quaternion.identity, team.transform);
            WormSpawned?.Invoke(newWorm);
            worms.Add(newWorm);
        }

        team.Init(worms, teamColor);
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
                edges.Add(newEdge);
        }
    }

    private Vector2 GetRandomPointForSpawn()
    {
        int random = Random.Range(0, edges.Count);
        var randomEdge = edges[random];
        Vector2 randomPoint = Vector2.Lerp(randomEdge.Point1, randomEdge.Point2, Random.value);
        randomPoint.y += _wormTemplate.Collider2D.size.y;
        return randomPoint;
    }
}
