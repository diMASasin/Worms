using BattleStateMachineComponents.StatesData;
using CameraFollow;
using Configs;
using Timers;
using UI_.Message;
using Water;
using Wind_;
using WormComponents;

namespace BattleStateMachineComponents.States
{
    public class BetweenTurnsState : IBattleState
    {
        private readonly IBattleStateSwitcher _battleStateSwitcher;
        private readonly ICurrentWorm _currentWorm;
        private readonly WindMediator _windMediator;
        private readonly ITimer _timer;
        private readonly TimersConfig _timersConfig;
        private readonly IFollowingCamera _followingCamera;
        private readonly WaterLevelIncreaser _waterLevelIncreaser;
        private readonly WaterVelocityChanger _waterVelocityChanger;
        private readonly IMessageShower _messageShower;
        private CycledList<Team> _aliveTeams;

        public BetweenTurnsState(IBattleStateSwitcher battleStateSwitcher, BattleStateMachineData data, 
            WindMediator windMediator, ITimer timer, ICurrentWorm currentWorm, IFollowingCamera followingCamera,
            WaterVelocityChanger waterVelocityChanger, IMessageShower messageShower)
        {
            _messageShower = messageShower;
            _waterVelocityChanger = waterVelocityChanger;
            _waterLevelIncreaser = data.WaterLevelIncreaser;
            _followingCamera = followingCamera;
            _timersConfig = data.BattleConfig.TimersConfig;
            _currentWorm = currentWorm;
            _battleStateSwitcher = battleStateSwitcher;
            _windMediator = windMediator;
            
            _timer = timer;
        }

        public void Enter()
        {
            _timer.Start(_timersConfig.BetweenTurnsDuration, () => _battleStateSwitcher.SwitchState<TurnState>());

            _followingCamera.RemoveAllTargets();
            _followingCamera.MoveToGeneralView();

            _windMediator.ChangeVelocity();
            _waterVelocityChanger.ChangeVelocity(_windMediator.Wind.NormalizedVelocity);
            _waterLevelIncreaser.IncreaseLevelIfAllowed();
            _messageShower.AppearGetReadyText();

            if (_currentWorm.CurrentWorm != null) _currentWorm.CurrentWorm.SetWormLayer();
        }

        public void Exit()
        {
            _timer.Stop();
        }
    }
}