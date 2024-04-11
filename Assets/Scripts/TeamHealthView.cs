using System.Collections.Generic;
using UnityEngine;

public class TeamHealthView : MonoBehaviour
{
    [SerializeField] private TeamHealth _teamHealthTemplate;
    [SerializeField] private Transform _container;

    private Bootstrap _bootstrap;
    
    public void Init(Bootstrap bootstrap)
    {
        _bootstrap = bootstrap;

        _bootstrap.WormsSpawned += OnWormsSpawned;
    }

    private void OnDestroy()
    {
        _bootstrap.WormsSpawned -= OnWormsSpawned;
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
