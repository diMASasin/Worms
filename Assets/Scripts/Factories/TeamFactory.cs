using System;
using System.Collections.Generic;
using Configs;
using EventProviders;
using UnityEngine;
using WormComponents;

namespace Factories
{
    public class TeamFactory : ITeamDiedEventProvider
    {
        private readonly WormFactory _wormFactory;
        
        public event Action<Team> TeamDied;

        public TeamFactory(WormFactory wormFactory)
        {
            _wormFactory = wormFactory;
        }

        public Team Create(Color color, Transform parent, TeamConfig config)
        {
            var teamWorms = new CycledList<Worm>();
            List<WormConfig> wormConfigs = config.WormConfigs;
            
            for (int i = 0; i < wormConfigs.Count; i++)
            {
                Worm newWorm = _wormFactory.Create(parent, color, wormConfigs[i]);
                teamWorms.Add(newWorm);
            }

            var team = new Team(teamWorms, color, config);
            team.Died += OnDied;
            
            return team;
        }

        private void OnDied(Team team)
        {
            team.Died -= OnDied;
            TeamDied?.Invoke(team);
        }
    }
}