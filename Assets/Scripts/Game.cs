using System;
using System.Collections;
using System.Collections.Generic;
using Configs;
using UnityEngine;
using UnityEngine.Events;

public class Game : IDisposable
{
    private int _currentTeamIndex = -1;
    private readonly List<Team> _currentTeams;
    private readonly ICoroutinePerformer _performer;
    private readonly List<Weapon> _weaponsList;
    private readonly Timer _afterShotTimer;
    private readonly Timer _turnTimer;
    private readonly Timer _globalTimer;
    private readonly TimerView _timerView;
    private readonly Wind _wind;
    private readonly TimersConfig _timersConfig;
    private readonly Water _water;

    private bool _isGlobalTimerElapsed;

    public event UnityAction TurnStarted;
    public event UnityAction TurnEnd;

    public Game(List<Team> currentTeams, ICoroutinePerformer performer, List<Weapon> weaponsList, TimersConfig timersConfig, 
        TimerView timerView, Water water, TimerView globalTimerView, Wind wind)
    {
        _currentTeams = currentTeams;
        _performer = performer;
        _weaponsList = weaponsList;
        _timersConfig = timersConfig;
        _water = water;
        _wind = wind;

        _afterShotTimer = new Timer();
        _turnTimer = new Timer();
        _globalTimer = new Timer();

        timerView.Init(_turnTimer, TimerFormattingStyle.Seconds);
        globalTimerView.Init(_globalTimer, TimerFormattingStyle.MinutesAndSeconds);

        TurnStarted += OnTurnStarted;
        TurnEnd += OnTurnEnd;
        
        foreach (var weapon in _weaponsList)
            weapon.Shot += OnShot;

        _globalTimer.Start(_timersConfig.GlobalTime, OnGlobalTimerElapsed);
    }

    public void Dispose()
    {
        TurnStarted -= OnTurnStarted;
        TurnEnd -= OnTurnEnd;
        
        foreach (var weapon in _weaponsList)
            weapon.Shot -= OnShot;
    }

    public void Tick()
    {
        _afterShotTimer.Tick();
        _turnTimer.Tick();
        _globalTimer.Tick();
    }

    public Team TryGetCurrentTeam()
    {
        if (_currentTeamIndex >= _currentTeams.Count || _currentTeamIndex < 0)
            return null;

        return _currentTeams[_currentTeamIndex];
    }

    public void NextTurn(float delay = 0)
    {
        EndTurn();
        _performer.StartRoutine(WaitUntilProjectilesExplode(() =>
        {
            _performer.StartRoutine(DelayedStartNextTurn(delay));
        }));
    }

    private void OnGlobalTimerElapsed()
    {
        _isGlobalTimerElapsed = true;
    }

    private void OnTurnStarted()
    {
        _turnTimer.Start(_timersConfig.TurnDuration, () => NextTurn(_timersConfig.AfterTurnWaitingDuration));
        _wind.ChangeVelocity();
    }

    private void OnTurnEnd()
    {
        if (_isGlobalTimerElapsed)
            _water.IncreaseLevel();
    }

    private void OnShot(Projectile projectile)
    {
        _afterShotTimer.Start(_timersConfig.AfterShotDuration, () => NextTurn(_timersConfig.AfterTurnWaitingDuration));
    }

    private void EndTurn()
    {
        var currentWorm = TryGetCurrentTeam()?.TryGetCurrentWorm();

        if (currentWorm == null) return;

        if (currentWorm.Weapon?.CurrentShotPower > 0)
            currentWorm.Weapon.Shoot();

        currentWorm.OnTurnEnd();
    }

    private IEnumerator WaitUntilProjectilesExplode(Action action)
    {
        while (ProjectilePool.Count > 0)
            yield return null;

        action();
        TurnEnd?.Invoke();
    }

    private IEnumerator DelayedStartNextTurn(float delay)
    {
      if (_currentTeams.Count <= 1) yield break;

        yield return new WaitForSeconds(delay);

        _currentTeamIndex++;
        if (_currentTeamIndex >= _currentTeams.Count)
            _currentTeamIndex = 0;

        TryGetCurrentTeam().StartTurn();
        TurnStarted?.Invoke();
    }
}