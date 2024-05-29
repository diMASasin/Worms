using BattleStateMachineComponents.StatesData;
using CameraFollow;
using Configs;
using Timers;
using Wind_;

namespace BattleStateMachineComponents.States
{
    public class BetweenTurnsState : IBattleState
    {
        private readonly IStateSwitcher _stateSwitcher;
        private readonly GlobalBattleData _data;
        private readonly BetweenTurnsStateData _betweenTurnsData;
        private TimersConfig TimersConfig => _data.TimersConfig;
        private Timer TurnTimer => _data.TurnTimer;
        private FollowingCamera FollowingCamera => _data.FollowingCamera;

        public BetweenTurnsState(IStateSwitcher stateSwitcher, GlobalBattleData data, 
            BetweenTurnsStateData betweenTurnsData)
        {
            _stateSwitcher = stateSwitcher;
            _data = data;
            _betweenTurnsData = betweenTurnsData;
        }

        public void Enter()
        {
            TurnTimer.Start(TimersConfig.BetweenTurnsDuration, 
                () => _stateSwitcher.SwitchState<TurnState>());

            FollowingCamera.ZoomTarget();
            FollowingCamera.SetTarget(_data.FollowingCamera.GeneralViewPosition);
            
            _betweenTurnsData.WindMediator.ChangeVelocity();
            _betweenTurnsData.Water.IncreaseLevelIfAllowed();
            
            if(_data.CurrentWorm != null)
                _data.CurrentWorm.SetWormLayer();
        }

        public void Exit()
        {
            TurnTimer.Stop();
        }

        public void Tick()
        {
        }
        
        public void FixedTick()
        {
            _betweenTurnsData.WindMediator.FixedTick();
        }

        public void HandleInput()
        {
        }
    }
}