using System;

public class WaterMediator : IDisposable
{
    private readonly Water _water;
    private readonly TimerMediator _timerMediator;
    private readonly Game _game;

    private bool _shouldIncreaseLevel;

    public WaterMediator(Water water, TimerMediator timerMediator, Game game)
    {
        _water = water;
        _timerMediator = timerMediator;
        _game = game;

        _game.TurnEnd += OnTurnEnd;
        _timerMediator.GlobalTimerElapsed += OnGlobalTimerElapsed;
    }

    public void Dispose()
    {
        _game.TurnEnd -= OnTurnEnd;
        _timerMediator.GlobalTimerElapsed -= OnGlobalTimerElapsed;
    }

    private void OnTurnEnd()
    {
        if (_shouldIncreaseLevel)
            _water.IncreaseLevel();
    }

    private void OnGlobalTimerElapsed()
    {
        _shouldIncreaseLevel = true;
    }
}