using BattleStateMachineComponents.StatesData;
using CameraFollow;
using Configs;
using Timers;
using UnityEngine;
using Wind_;

namespace BattleStateMachineComponents.States
{
    public class BetweenTurnsState : IBattleState
    {
        private readonly IStateSwitcher _stateSwitcher;
        private readonly GlobalBattleData _data;
        private readonly BetweenTurnsStateData _betweenTurnsData;
        private readonly Timer _timer;
        private TimersConfig TimersConfig => _data.TimersConfig;
        private FollowingCamera FollowingCamera => _data.FollowingCamera;

        public BetweenTurnsState(IStateSwitcher stateSwitcher, GlobalBattleData data, 
            BetweenTurnsStateData betweenTurnsData)
        {
            _stateSwitcher = stateSwitcher;
            _data = data;
            _betweenTurnsData = betweenTurnsData;
            _timer = new Timer();
        }

        public void Enter()
        {
            _timer.Start(TimersConfig.BetweenTurnsDuration, 
                () => _stateSwitcher.SwitchState<TurnState>());

            // FollowingCamera.ZoomTarget();
            FollowingCamera.SetTarget(_data.FollowingCamera.GeneralViewPosition);
            
            _betweenTurnsData.WindMediator.ChangeVelocity();
            _data.Water.IncreaseLevelIfAllowed();

            _data.CurrentWorm?.SetWormLayer();
            
            if (_data.AliveTeams.Count <= 1) 
                _stateSwitcher.SwitchState<BattleEndState>();
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
            _betweenTurnsData.WindMediator.FixedTick();
        }

        public void LateTick()
        {
        }

        public void HandleInput()
        {
        }
    }
}