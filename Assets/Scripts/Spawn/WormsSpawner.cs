using System.Collections.Generic;
using Configs;
using Factories;
using UnityEngine;
using WormComponents;
using Random = UnityEngine.Random;

namespace Spawn
{
    public class WormsSpawner
    {
        private readonly TeamFactory _teamFactory;
        
        private readonly List<Color> _unusedTeamColors = new();
        private readonly WormsSpawnerConfig _wormsSpawnerConfig;

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