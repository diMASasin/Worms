using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamHealthView : MonoBehaviour
{
    [SerializeField] private Game _game;
    [SerializeField] private TeamHealth _teamHealthTemplate;
    [SerializeField] private Transform _container;

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
        foreach (var team in teams)
        {
            var teamHealth = Instantiate(_teamHealthTemplate, _container);
            teamHealth.Init(team.Color, team);
        }
    }
}
