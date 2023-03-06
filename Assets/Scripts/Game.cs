using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Game : MonoBehaviour
{
    [SerializeField] private int _teamsNumber = 2;
    [SerializeField] private int _wormsNumber = 4;
    [SerializeField] private WormsSpawner _wormsSpawner;
    [SerializeField] private List<Color> _teamColors;
    [SerializeField] private float _turnDelay = 2.5f;
    [SerializeField] private GameObject _endScreen;
    [SerializeField] private FollowingCamera _followingCamera;

    private List<Team> _teams = new List<Team>();
    private List<Team> _currentTeams = new List<Team>();
    private int _currentTeamIndex = 0;

    public event UnityAction<List<Team>> WormsSpawned;

    private void OnEnable()
    {
        _wormsSpawner.WormSpawned += OnWormSpawned;
    }

    private void OnDisable()
    {
        _wormsSpawner.WormSpawned -= OnWormSpawned;
    }

    private void Start()
    {
        _wormsSpawner.GetEdgesForSpawn();
        _teams = _wormsSpawner.SpawnTeams(_teamsNumber, _wormsNumber, _teamColors);
        WormsSpawned?.Invoke(_teams);

        foreach (var team in _teams)
        {
            team.Died += OnTeamDied;
        }

        _currentTeams.AddRange(_teams);

        StartNextTurnWithDelay(0);
    }

    private void OnWormSpawned(Worm worm)
    {
        worm.Died += OnWormDied;
        worm.Throwing.ProjectileExploded += OnProjectileExploded;
    }

    private void OnProjectileExploded(Bomb bomb, Worm worm)
    {
        worm.WormInput.DisableInput();
        StartNextTurnWithDelay(_turnDelay);
    }

    private void OnWormDied(Worm worm)
    {
        worm.Died -= OnWormDied;
        worm.Throwing.ProjectileExploded -= OnProjectileExploded;

        if (worm.WormInput.enabled == true)
            StartNextTurnWithDelay(_turnDelay);
    }

    private void OnTeamDied(Team team)
    {
        _currentTeams.Remove(team);

        if (_currentTeams.Count <= 1)
            _endScreen.SetActive(true);
    }

    private void StartNextTurnWithDelay(float delay)
    {
        StartCoroutine(DelayedStartNextTurn(delay));
    }

    private IEnumerator DelayedStartNextTurn(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (_currentTeamIndex >= _currentTeams.Count)
            _currentTeamIndex = 0;

        _currentTeams[_currentTeamIndex].StartTurn();
        _currentTeamIndex++;
    }
}
