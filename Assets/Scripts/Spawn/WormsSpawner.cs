using System;
using System.Collections.Generic;
using Battle_;
using Configs;
using DestructibleLand;
using Factories;
using ScriptBoy.Digable2DTerrain.Scripts;
using UnityEngine;
using WormComponents;
using Zenject;
using Random = UnityEngine.Random;

namespace Spawn
{
    public class WormsSpawner
    {
        private TeamFactory _teamFactory;
        
        private readonly List<Color> _unusedTeamColors = new();
        private WormsSpawnerConfig _wormsSpawnerConfig;

        public WormsSpawner(WormsSpawnerConfig wormsSpawnerConfig, TeamFactory teamFactory)
        {
            _wormsSpawnerConfig = wormsSpawnerConfig;
            _teamFactory = teamFactory;
            
            _unusedTeamColors.AddRange(_wormsSpawnerConfig.TeamColors);
        }

        public CycledList<Team> Spawn(int teamsCount, int wormsCount)
        {
            Transform parent = new GameObject("Worms").transform;
            var teams = new CycledList<Team>();
            
            for (int i = 0; i < teamsCount; i++)
            {
                var teamConfig = _wormsSpawnerConfig.TeamConfigs[i];
                Color randomColor = _unusedTeamColors[Random.Range(0, _unusedTeamColors.Count)];
                _unusedTeamColors.Remove(randomColor);
                Team team = _teamFactory.Create(randomColor, parent, teamConfig, wormsCount);

                teams.Add(team);
            }

            return teams;
        }
    }
}