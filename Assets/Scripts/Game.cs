using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] private int _teamsNumber = 2;
    [SerializeField] private int _wormsNumber = 4;
    [SerializeField] private WormsSpawner _wormsSpawner;
    [SerializeField] private List<Color> _teamColors;

    private List<Team> _teams = new List<Team>();

    private void Start()
    {
        _wormsSpawner.GetEdgesForSpawn();
        _teams = _wormsSpawner.SpawnTeams(_teamsNumber, _wormsNumber, _teamColors);

        _teams[0].StartTurn();
    }
}
