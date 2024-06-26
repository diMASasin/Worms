using Battle_;
using CameraFollow;
using Configs;
using Factories;
using Spawn;
using Timers;
using UltimateCC;
using UnityEngine;
using Zenject;

namespace BattleStateMachineComponents.States
{
    public class BootstrapBattleState : IBattleState
    {
        private IBattleStateSwitcher _battleStateSwitcher;
        private readonly BattleStateMachineData _data;
        private readonly WeaponBootstrapper _weaponBootstrapper;
        private readonly ProjectileInstaller _projectileInstaller;
        private readonly WormsBootstraper _wormsBootstraper;
        private DiContainer _container;
        private Transform _healthParent;
        private TeamHealthFactory _teamHealthFactoryPrefab;
        private WormsSpawner _wormsSpawner;
        private IBattleSettings _battleSettings;
        private Timer _battleTimer;
        private WeaponSelectorItemFactory _itemFactory;
        private WeaponFactory _weaponFactory;

        private BattleConfig BattleConfig => _data.BattleConfig;

        public BootstrapBattleState(BattleStateMachineData data, DiContainer container, 
            IBattleStateSwitcher battleStateSwitcher, WormsSpawner wormsSpawner, IBattleSettings battleSettings, 
            Timer battleTimer, Timer turnBattle, WeaponSelectorItemFactory itemFactory, WeaponFactory weaponFactory)
        {
            _weaponFactory = weaponFactory;
            _itemFactory = itemFactory;
            _battleTimer = battleTimer;
            _data = data;
            _battleSettings = battleSettings;
            _healthParent = _data.UIChanger.transform;
            _teamHealthFactoryPrefab = _data.BattleConfig.TeamHealthFactoryPrefab;
            _wormsSpawner = wormsSpawner;
            _container = container;
            _battleStateSwitcher = battleStateSwitcher;
            
            _data.BattleTimer = battleTimer;
            _data.TurnTimer = turnBattle;
        }

        public void Enter()
        {
            var weaponList = _weaponFactory.Create();
            _itemFactory.Create(weaponList);   
            
            SettingsData settingsData = _battleSettings.Data;
            _data.Terrain.GetEdgesForSpawn();
            _data.AliveTeams = _wormsSpawner.Spawn(settingsData.TeamsCount, settingsData.WormsCount);
            
            CreateTeamHealthFactory();

            InitializeTimers();
            
            _battleStateSwitcher.SwitchState<BetweenTurnsState>();
        }

        private void CreateTeamHealthFactory()
        {
            var teamHealthFactory =
                _container.InstantiatePrefabForComponent<TeamHealthFactory>(_teamHealthFactoryPrefab, _healthParent);
            teamHealthFactory.Create(_data.AliveTeams, BattleConfig.TeamHealthPrefab);
            teamHealthFactory.transform.SetAsFirstSibling();
        }

        private void InitializeTimers()
        {
            float globalTime = BattleConfig.TimersConfig.BattleTime;
            
            _battleTimer.Start(globalTime, () => _data.Water.AllowIncreaseWaterLevel());
            _battleTimer.Pause();

            _data.GlobalTimerView.Init(_battleTimer, TimerFormattingStyle.MinutesAndSeconds);
            _data.TurnTimerView.Init(_data.TurnTimer, TimerFormattingStyle.Seconds);
        }

        public void Exit() { }

        public void Tick(){}

        public void FixedTick(){ }
    }
}