using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Game : MonoBehaviour
{
    [SerializeField] private WormsSpawner _wormsSpawner;
    [SerializeField] private float _turnDelay = 2.5f;
    [SerializeField] private EndScreen _endScreen;

    private List<Team> _teams;
    private int _currentTeamIndex = -1;

    private readonly List<Team> _currentTeams = new();

    public event UnityAction<List<Team>> WormsSpawned;
    public event UnityAction NextTurnStarted;
    public event UnityAction TurnEnd;

    private void Start()
    {
        _wormsSpawner.WormSpawned += OnWormSpawned;

        _wormsSpawner.GetEdgesForSpawn();
        _teams = _wormsSpawner.SpawnTeams();
        _currentTeams.AddRange(_teams);
        
        foreach (var team in _teams)
            team.Died += OnTeamDied;

        WormsSpawned?.Invoke(_teams);
        StartNextTurnWithDelay(0);
    }

    private void OnDestroy()
    {
        _wormsSpawner.WormSpawned -= OnWormSpawned;

        foreach (var team in _teams)
            team.Died -= OnTeamDied;
    }

    private void OnWormSpawned(Worm worm)
    {
        worm.Died += OnWormDied;
    }

    public Team TryGetCurrentTeam()
    {
        if (_currentTeamIndex >= _currentTeams.Count)
            return null;

        return _currentTeams[_currentTeamIndex];
    }

    public void DisableCurrentWorm()
    {
        var currentWorm = _currentTeams[_currentTeamIndex].TryGetCurrentWorm();

        currentWorm.RemoveWeaponWithDelay();
        currentWorm.Input.DisableInput();
    }

    private void OnWormDied(Worm worm)
    {
        worm.Died -= OnWormDied;

        worm.RemoveWeapon();

        if (worm.Input.IsEnabled == true)
            StartNextTurnWithDelay(_turnDelay);
    }

    private void OnTeamDied(Team team)
    {
        _currentTeams.Remove(team);

        if (_currentTeams.Count <= 1)
            _endScreen.gameObject.SetActive(true);
    }

    public IEnumerator WaitUntilProjectilesExplode(Action action)
    {
        while (ProjectilePool.Count > 0)
            yield return null;

        yield return new WaitForSeconds(0.5f);

        action();
        TurnEnd?.Invoke();
    }

    public void StartNextTurnWithDelay(float delay)
    {
        if (_currentTeams.Count == 0)
            return;

        StartCoroutine(DelayedStartNextTurn(delay));
    }

    private IEnumerator DelayedStartNextTurn(float delay)
    {
        yield return new WaitForSeconds(delay);

        _currentTeamIndex++;
        if (_currentTeamIndex >= _currentTeams.Count)
            _currentTeamIndex = 0;

        if (_currentTeams.Count <= 1) yield break;

        _currentTeams[_currentTeamIndex].StartTurn();
        NextTurnStarted?.Invoke();
    }
}