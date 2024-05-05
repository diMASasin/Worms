using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Game : IDisposable
{
    private int _currentTeamIndex = -1;
    private readonly List<Team> _currentTeams;

    public Worm CurrentWorm { get; private set; }
    public Team CurrentTeam { get; private set; }

    public event UnityAction<Worm, Team> TurnStarted;
    public event UnityAction TurnEnd;
    public event Action GameStarted;
    public event Action GameEnd;

    public Game(List<Team> currentTeams)
    {
        _currentTeams = currentTeams;

        foreach (var team in _currentTeams)
            team.Died += OnTeamDied;
    }

    public void Dispose()
    {
        foreach (var team in _currentTeams)
            team.Died -= OnTeamDied;
    }

    public void StartGame()
    {
        GameStarted?.Invoke();
    }

    public void StartNextTurn(float delay = 0)
    {
        EndTurn();
        CoroutinePerformer.StartRoutine(WaitUntilProjectilesExplode(() =>
        {
            CoroutinePerformer.StartRoutine(DelayedStartNextTurn(delay));
        }));
    }

    private void OnTeamDied(Team team)
    {
        if (_currentTeams.Count <= 1)
            GameEnd?.Invoke();
    }

    private void EndTurn()
    {
        if (CurrentWorm == null) return;

        if (CurrentWorm.Weapon?.CurrentShotPower > 0)
            CurrentWorm.Weapon.Shoot();

        CurrentWorm.OnTurnEnd();
    }

    private IEnumerator WaitUntilProjectilesExplode(Action action)
    {
        while (ProjectilePool.Count > 0)
            yield return null;

        TurnEnd?.Invoke();
        action();
    }

    public bool TryGetCurrentTeam(out Team team)
    {
        if (_currentTeamIndex >= _currentTeams.Count || _currentTeamIndex < 0)
            team = null;
        else
            team = _currentTeams[_currentTeamIndex];

        return team != null;
    }

    private bool TryGetNextTeam(out Team team)
    {
        _currentTeamIndex++;

        if (_currentTeamIndex >= _currentTeams.Count)
            _currentTeamIndex = 0;

        TryGetCurrentTeam(out team);

        return team != null;
    }

    private IEnumerator DelayedStartNextTurn(float delay)
    {
      if (_currentTeams.Count <= 1) yield break;

        yield return new WaitForSeconds(delay);

        TryGetNextTeam(out Team team);
        team.TryGetNextWorm(out Worm worm);
        worm.OnTurnStarted();

        CurrentWorm = worm;
        CurrentTeam = team;
        
        TurnStarted?.Invoke(worm, team);
    }
}