using System;
using System.Collections;
using System.Collections.Generic;
using Configs;
using EventProviders;
using Pools;
using UnityEngine;
using GameBattleStateMachine;
using GameBattleStateMachine.States;

public class Game : IDisposable, IGameEventsProvider
{
    private int _currentTeamIndex = -1;
    private readonly List<Team> _currentTeams = new();
    private readonly ITeamDiedEventProvider _teamDiedEvent;
    private readonly TimersConfig _timersConfig;
    private BattleStateMachine _battleStateMachine; 

    public Worm CurrentWorm { get; private set; }
    public Team CurrentTeam { get; private set; }

    public event Action <Worm, Team> TurnStarted;
    public event Action TurnEnd;
    public event Action GameStarted;
    public event Action GameEnd;

    public Game(IEnumerable<Team> teams, ITeamDiedEventProvider teamDiedEvent, TimersConfig timersConfig)
    {
        _currentTeams.AddRange(teams);
        
        _teamDiedEvent = teamDiedEvent;
        _timersConfig = timersConfig;
        
        _battleStateMachine = new BattleStateMachine(timersConfig);
        
        _teamDiedEvent.TeamDied += OnTeamDied;
    }

    public void Dispose()
    {
        _teamDiedEvent.TeamDied -= OnTeamDied;
    }

    public void Tick()
    {
        _battleStateMachine.Tick();
    }
    
    public void StartGame()
    {
        _battleStateMachine.SwitchState<BetweenTurnsState>();
        GameStarted?.Invoke();
    }

    public void StartNextTurn(float delay = 0)
    {
        EndTurn();
        CoroutinePerformer.StartCoroutine(WaitUntilProjectilesExplode(() =>
        {
            CoroutinePerformer.StartCoroutine(DelayedStartNextTurn(delay));
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