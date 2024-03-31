using System;
using System.Collections.Generic;

public class TurnTimer : Timer, IDisposable
{
    private readonly float _afterTurnInterval;
    private readonly WeaponSelector _weaponSelector;
    private readonly Game _game;

    public event Action TimerOut;
    public event Action WormShot;
    public event Action TimerStopped;

    public TurnTimer(Game game, WeaponSelector weaponSelector, float afterTurnInterval)
    {
        _game = game;
        _weaponSelector = weaponSelector;
        _afterTurnInterval = afterTurnInterval;

        _game.WormsSpawned += OnWormsSpawned;
        Elapsed += OnElapsed;

        foreach (var weapon in _weaponSelector.WeaponList)
            weapon.Shot += OnShot;
    }

    public void Dispose()
    {
        Elapsed -= OnElapsed;

        _game.WormsSpawned -= OnWormsSpawned;

        foreach (var weapon in _weaponSelector.WeaponList)
            weapon.Shot -= OnShot;
    }

    private void OnWormsSpawned(List<Team> teams)
    {
        foreach(var team in teams)
        {
            team.TurnStarted += OnTurnStarted;
            team.Died += OnDied;
        }
    }

    private void OnDied(Team team)
    {
        team.TurnStarted -= OnTurnStarted;
        team.Died -= OnDied;
    }

    private void OnTurnStarted(Worm worm, Team team)
    {
        Start(_afterTurnInterval);
    }

    private void OnElapsed()
    {
        var currentWorm = _game.TryGetCurrentTeam().TryGetCurrentWorm();
        if (currentWorm.Weapon?.CurrentShotPower > 0)
        {
            currentWorm.Weapon.Shoot();
            WormShot?.Invoke();
            return;
        }

        _game.DisableCurrentWorm();
        _game.StartNextTurnWithDelay(1);
        TimerOut?.Invoke();
    }

    private void OnShot(Projectile projectile)
    {
        Stop();
        TimerStopped?.Invoke();
    }
}
