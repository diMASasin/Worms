using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterTurnTimer : MonoBehaviour
{
    [SerializeField] private float _time;
    [SerializeField] private TurnTimer _turnTimer;
    [SerializeField] private Timer _timer;
    [SerializeField] private Game _game;

    private void OnValidate()
    {
        _game = FindObjectOfType<Game>();
    }

    private void OnEnable()
    {
        _turnTimer.WormShot += OnShot;
        _turnTimer.TimerStopped += OnShot;
    }

    private void OnDisable()
    {
        _turnTimer.WormShot -= OnShot;
        _turnTimer.TimerStopped -= OnShot;
    }

    private void OnShot()
    {
        StartTimer();
    }

    private void StartTimer()
    {
        _timer.StartTimer(_time, OnTimerOut);
    }

    private void OnTimerOut()
    {
        _timer.StopTimer();
        _game.DisableCurrentWorm();
        //_game.StartNextTurnWithDelay(1.5f);
        StartCoroutine(_game.WaitUntilProjectilesExplode(() => _game.StartNextTurnWithDelay(1)));
    }
}
