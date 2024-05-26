using System;
using System.Collections.Generic;
using Battle_;
using Configs;
using DestructibleLand;
using Factories;
using ScriptBoy.Digable2DTerrain.Scripts;
using UnityEngine;
using WormComponents;
using Random = UnityEngine.Random;

namespace Spawn
{
    public class WormsSpawner : MonoBehaviour
    {
        [SerializeField] private WormsSpawnerConfig _spawnerConfig;

        private TeamFactory _teamFactory;
        
        private readonly List<Color> _unusedTeamColors = new();

        public void Init(TeamFactory teamFactory)
        {
            _teamFactory = teamFactory;
            _unusedTeamColors.AddRange(_spawnerConfig.TeamColors);
        }

        public CycledList<Team> Spawn(int teamsNumber, int wormsNumber)
        {
            var teams = new CycledList<Team>();

            for (var i = 0; i < teamsNumber; i++)
            {
                var teamConfig = _spawnerConfig.TeamConfigs[i];
                Color randomColor = _unusedTeamColors[Random.Range(0, _unusedTeamColors.Count)];
                _unusedTeamColors.Remove(randomColor);
                Team team = _teamFactory.Create(randomColor, transform, teamConfig, wormsNumber);

                teams.Add(team);
            }

            return teams;
        }
    }
}