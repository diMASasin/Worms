using System;

public class AfterTurnTimer : Timer, IDisposable
{
    private readonly float _interval;
    private readonly Game _game;
    private readonly TurnTimer _turnTimer;
    private readonly ICoroutinePerformer _performer;

    public AfterTurnTimer(TurnTimer turnTimer, Game game, ICoroutinePerformer performer, float afterTurnInterval)
    {
        _turnTimer = turnTimer;
        _game = game;
        _performer = performer;
        _interval = afterTurnInterval;
        
        _turnTimer.WormShot += OnShot;
        Elapsed += OnElapsed;
    }

    public void Dispose()
    {
        _turnTimer.WormShot -= OnShot;
        Elapsed -= OnElapsed;
    }

    private void OnShot()
    {
        Start(_interval);
    }

    private void OnElapsed()
    {
        Stop();
        _game.DisableCurrentWorm();
        _performer.StartRoutine(_game.WaitUntilProjectilesExplode(() => _game.StartNextTurnWithDelay(1)));
    }
}
