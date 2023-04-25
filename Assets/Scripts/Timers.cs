using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timers : MonoBehaviour
{
    [SerializeField] private Game _game;
    [SerializeField] private Timer _timer;
    private List<Team> _teams;

    private void OnEnable()
    {
        _game.WormsSpawned += OnWormsSpawned; 
    }

    private void OnDisable()
    {
        _game.WormsSpawned -= OnWormsSpawned;
    }

    private void OnWormsSpawned(List<Team> teams)
    {
        foreach(var team in teams)
        {
            team.TurnStarted += OnTurnStarted;
            team.Died += OnDied;
        }
    }

    private void OnTurnStarted(Worm worm, Team team)
    {
        _timer.StartTimer();
    }

    private void OnDied(Team team)
    {
        team.TurnStarted -= OnTurnStarted;
        team.Died -= OnDied;
    }
}
