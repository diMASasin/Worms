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
        private readonly ProjectilesBootsrapper _projectilesBootsrapper;
        private readonly WormsBootstraper _wormsBootstraper;
        private DiContainer _container;
        private Transform _healthParent;
        private TeamHealthFactory _teamHealthFactoryPrefab;
        private WormsSpawner _wormsSpawner;
        private IBattleSettings _battleSettings;
        private Timer _battleTimer;
        private Timer _turnBattle;
        private IMovementInput _movementInput;
        private WeaponSelectorItemFactory _itemFactory;
        private WeaponFactory _weaponFactory;

        private GameConfig GameConfig => _data.GameConfig;

        public BootstrapBattleState(BattleStateMachineData data, DiContainer container, 
            IBattleStateSwitcher battleStateSwitcher, WormsSpawner wormsSpawner, IBattleSettings battleSettings, 
            Timer battleTimer, Timer turnBattle, IMovementInput movementInput, WeaponSelectorItemFactory itemFactory, WeaponFactory weaponFactory)
        {
            _weaponFactory = weaponFactory;
            _itemFactory = itemFactory;
            _movementInput = movementInput;
            _turnBattle = turnBattle;
            _battleTimer = battleTimer;
            _data = data;
            _battleSettings = battleSettings;
            _healthParent = _data.UIChanger.transform;
            _teamHealthFactoryPrefab = _data.GameConfig.TeamHealthFactoryPrefab;
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
            
            var teamHealthFactory = 
                _container.InstantiatePrefabForComponent<TeamHealthFactory>(_teamHealthFactoryPrefab, _healthParent);
            teamHealthFactory.Create(_data.AliveTeams, GameConfig.TeamHealthPrefab);
            teamHealthFactory.transform.SetAsFirstSibling();
            
            InitializeBootstrappers();

            InitializeTimers();
            
            _battleStateSwitcher.SwitchState<BetweenTurnsState>();
        }

        private void InitializeBootstrappers()
        {
            
        }

        private void InitializeTimers()
        {
            float globalTime = GameConfig.TimersConfig.BattleTime;
            
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