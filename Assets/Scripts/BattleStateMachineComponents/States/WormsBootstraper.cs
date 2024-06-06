using System;
using Battle_;
using BattleStateMachineComponents.StatesData;
using Configs;
using DestructibleLand;
using EventProviders;
using Factories;
using Spawn;
using UnityEngine;
using WormComponents;
using Object = UnityEngine.Object;

namespace BattleStateMachineComponents.States
{
    public class WormsBootstraper : IDisposable
    {
        private readonly TerrainWrapper _terrain;
        private readonly IBattleSettings _battleSettings;
        private readonly GameConfig _gameConfig;
        private readonly Transform _teamHealthParent;
        private readonly CycledList<Team> _aliveTeams;
        private WormInfoFactory _wormInfoFactory;
        private WormsSpawner _wormsSpawner;
        private WormFactory _wormFactory;

        public ITeamDiedEventProvider TeamDiedEvent { get; private set; }

        public WormsBootstraper(TerrainWrapper terrain, IBattleSettings battleSettings, 
            GameConfig gameConfig, Transform teamHealthParent, CycledList<Team> aliveTeams)
        {
            _terrain = terrain;
            _battleSettings = battleSettings;
            _gameConfig = gameConfig;
            _teamHealthParent = teamHealthParent;
            _aliveTeams = aliveTeams;
        }

        public void Dispose()
        {
            _wormInfoFactory.Dispose();
            _wormFactory.Dispose();
        }

        public void SpawnWorms(out WormFactory wormFactory)
        {
            int teamsNumber = _battleSettings.Data.TeamsCount;
            int wormsNumber = _battleSettings.Data.WormsCount;

            _wormFactory = wormFactory = new WormFactory(_gameConfig.WormPrefab, _terrain);
            var teamFactory = new TeamFactory(wormFactory);
            _wormInfoFactory = new WormInfoFactory(_gameConfig.WormInfoViewPrefab, wormFactory);

            _wormsSpawner.Init(teamFactory);
            TeamDiedEvent = teamFactory;
            
            CycledList<Team> teams = _wormsSpawner.Spawn(teamsNumber, wormsNumber);

            _aliveTeams.AddRange(teams);

            TeamHealthFactory teamHealthFactory = Object.Instantiate(_gameConfig.TeamHealthFactoryPrefab, _teamHealthParent);
            teamHealthFactory.Create(_aliveTeams, _gameConfig.TeamHealthPrefab);
        }
    }
}