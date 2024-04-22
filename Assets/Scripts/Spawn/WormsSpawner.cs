using ScriptBoy.Digable2DTerrain;
using System;
using System.Collections.Generic;
using Configs;
using Spawn;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class WormsSpawner : MonoBehaviour
{
    [SerializeField] private SpawnerData _spawnerData;

    [SerializeField] private Terrain2D _terrain;
    [SerializeField] private Worm _wormTemplate;
    [SerializeField] private float _maxSlope = 45;
    [SerializeField] private MapBounds _mapBounds;

    private Vector2[] _points;
    private readonly List<Edge> _edges = new();
    private readonly List<Worm> _wormsList = new();
    private List<Worm> _teamWorms;
    private GroundCheckerConfig _config;

    public List<Worm> WormsList => _wormsList;
    public int TeamsNumber => _spawnerData.TeamsNumber;
    public int WormsNumber=> _spawnerData.WormsNumber;
    public List<Color> TeamColors => _spawnerData.TeamColors;

    private const string TEAMS_NUMBER = nameof(TEAMS_NUMBER);
    private const string WORMS_NUMBER = nameof(WORMS_NUMBER);

    public event UnityAction<Worm> WormSpawned;

    private void Awake()
    {
        _spawnerData.WormsNumber = PlayerPrefs.GetInt(TEAMS_NUMBER, TeamsNumber);
        _spawnerData.TeamsNumber = PlayerPrefs.GetInt(WORMS_NUMBER, WormsNumber);
    }

    public List<Team> SpawnTeams()
    {
        if(TeamColors.Count < TeamsNumber)
            throw new ArgumentOutOfRangeException($"{nameof(TeamColors)} count cant be less then {nameof(TeamsNumber)}");

        List<Team> teams = new();

        for (int i = 0; i < TeamsNumber; i++)
        {
            var randomColor = TeamColors[Random.Range(0, TeamColors.Count)];
            TeamColors.Remove(randomColor);

            var newTeam = SpawnTeam(WormsNumber, randomColor, $"Team {i + 1}");
            teams.Add(newTeam);
        }
        return teams;
    }

    private Team SpawnTeam(int wormsNumber, Color teamColor, string teamName)
    {
         _teamWorms = new List<Worm>();

        for (int i = 0; i < wormsNumber; i++)
        {
            var newWorm = Instantiate(_wormTemplate, GetRandomPointForSpawn(), Quaternion.identity, transform);

            newWorm.Init(teamColor, $"Worm {i + 1}");
            newWorm.SetRigidbodyKinematic();

            WormSpawned?.Invoke(newWorm);
            _teamWorms.Add(newWorm);
        }

        _wormsList.AddRange(_teamWorms);
        var team = new Team(_teamWorms, teamColor, teamName);

        return team;
    }

    public void GetEdgesForSpawn()
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

            if (newEdge.IsFloor(_terrain) && newEdge.IsSuitableSlope(_maxSlope) && edgeInBounds)
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
            randomPoint += (Vector2)_terrain.transform.position;
        }
        while (!CanFitWormInPosition(randomPoint));

        return randomPoint;
    }

    private bool CanFitWormInPosition(Vector2 position)
    {
        var size = new Vector2(_wormTemplate.Collider2D.size.x * 2, _wormTemplate.Collider2D.size.y);
        return !Physics2D.OverlapCapsule(position + new Vector2(0, 0.5f), size, CapsuleDirection2D.Vertical, 0);
    }
}
