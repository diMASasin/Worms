using CameraFollow;
using Configs;
using Timers;
using Wind_;
using WormComponents;

namespace BattleStateMachineComponents.States
{
    public class BetweenTurnsState : IBattleState
    {
        private readonly IBattleStateSwitcher _battleStateSwitcher;
        private readonly BattleStateMachineData _data;
        private readonly Worm _currentWorm;
        private readonly WindMediator _windMediator;
        private readonly Timer _timer;
        private readonly TimersConfig _timersConfig;
        private readonly FollowingCamera _followingCamera;
        private readonly Water _water;
        private CycledList<Team> _aliveTeams;

        public BetweenTurnsState(IBattleStateSwitcher battleStateSwitcher, BattleStateMachineData data, WindMediator windMediator, Timer timer)
        {
            _water = data.Water;
            _followingCamera = data.FollowingCamera;
            _timersConfig = data.TimersConfig;
            _currentWorm = data.CurrentWorm;
            _battleStateSwitcher = battleStateSwitcher;
            _data = data;
            _windMediator = windMediator;
            
            _timer = timer;
        }

        public void Enter()
        {
            _timer.Start(_timersConfig.BetweenTurnsDuration, () => _battleStateSwitcher.SwitchState<TurnState>());

            _followingCamera.SetTarget(_followingCamera.GeneralViewPosition);
            
            _windMediator.ChangeVelocity();
            _water.IncreaseLevelIfAllowed();

            if (_currentWorm != null) _currentWorm.SetWormLayer();

            if (_data.AliveTeams.Count <= 1) 
                _battleStateSwitcher.SwitchState<BattleEndState>();
        }

        public void Exit()
        {
            _timer.Stop();
        }

        public void Tick()
        {
        }
        
        public void FixedTick()
        {
            _windMediator.FixedTick();
        }

        public void LateTick()
        {
        }

        public void HandleInput()
        {
        }
    }
}