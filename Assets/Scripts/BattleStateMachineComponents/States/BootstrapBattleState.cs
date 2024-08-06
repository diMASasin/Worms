using Battle_;
using Configs;
using Factories;
using ScriptBoy.Digable2DTerrain.Scripts;
using Spawn;
using Timers;
using UI_;
using UnityEngine;
using Zenject;

namespace BattleStateMachineComponents.States
{
    public class BootstrapBattleState : IBattleState
    {
        private readonly IBattleStateSwitcher _battleStateSwitcher;
        private readonly BattleStateMachineData _data;
        private readonly WeaponBootstrapper _weaponBootstrapper;
        private readonly ProjectileInstaller _projectileInstaller;
        private readonly WormsBootstraper _wormsBootstraper;
        private TeamHealthFactory _teamHealthFactoryPrefab;
        private readonly WormsSpawner _wormsSpawner;
        private readonly IBattleSettings _battleSettings;
        private readonly WeaponSelectorItemFactory _itemFactory;
        private readonly WeaponFactory _weaponFactory;
        private readonly TeamHealthFactory _teamHealthFactory;
        private DiContainer _container;
        private LoadingScreen _loadingScreen;

        private BattleConfig BattleConfig => _data.BattleConfig;

        public BootstrapBattleState(BattleStateMachineData data, DiContainer container, IBattleStateSwitcher battleStateSwitcher,
            WormsSpawner wormsSpawner, IBattleSettings battleSettings, ITimer battleTimer, ITimer turnBattle,
            WeaponSelectorItemFactory itemFactory, WeaponFactory weaponFactory, LoadingScreen loadingScreen)
        {
            _loadingScreen = loadingScreen;
            _container = container;
            _weaponFactory = weaponFactory;
            _itemFactory = itemFactory;
            _data = data;
            _battleSettings = battleSettings;
            _wormsSpawner = wormsSpawner;
            _battleStateSwitcher = battleStateSwitcher;
        }

        public void Enter()
        {
            var weaponList = _weaponFactory.Create();
            _itemFactory.Create(weaponList);   
            
            SettingsData settingsData = _battleSettings.Data;
            _data.Terrain.GetEdgesForSpawn();
            _data.AliveTeams = _wormsSpawner.Spawn(settingsData.TeamsCount, settingsData.WormsCount);

            CreateShovel();
            CreateTeamHealth();
            InitializeTimers();
            
            _battleStateSwitcher.SwitchState<BetweenTurnsState>();
        }

        private void CreateShovel()
        {
            var shovel = _container.InstantiatePrefabForComponent<Shovel>(BattleConfig.ShovelPrefab);
        }

        private void CreateTeamHealth()
        {
            Transform teamHealthParent = _data.UI.transform;
            TeamHealthFactory teamHealthPrefab = BattleConfig.TeamHealthFactoryPrefab;
            
            var factory = _container.InstantiatePrefabForComponent<TeamHealthFactory>(teamHealthPrefab, teamHealthParent);

            factory.Create(_data.AliveTeams, BattleConfig.TeamHealthPrefab);
            factory.transform.SetAsFirstSibling();
        }

        private void InitializeTimers()
        {
            float globalTime = BattleConfig.TimersConfig.BattleTime;
            
            _data.BattleTimer.Start(globalTime, () => _data.WaterLevelIncreaser.AllowIncreaseWaterLevel());
            _data.BattleTimer.Pause();

            _data.UI.BattleTimerView.Init(_data.BattleTimer, new TimeSecondsAndMinutesFormatter());
            _data.UI.TurnTimerView.Init(_data.TurnTimer, new TimeSecondsFormatter());
        }

        public void Exit()
        {
            _loadingScreen.Disable();
        }
    }
}