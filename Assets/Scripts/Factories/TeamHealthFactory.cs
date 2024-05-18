using System.Collections.Generic;
using UnityEngine;
using WormComponents;

public class TeamHealthFactory : MonoBehaviour
{
    [SerializeField] private TeamHealth _teamHealthTemplate;
    [SerializeField] private Transform _container;
    
    public void Create(CycledList<Team> teams)
    {
        foreach (var team in teams)
        {
            var teamHealth = Instantiate(_teamHealthTemplate, _container);
            teamHealth.Init(team.Color, team);
        }
    }
}
