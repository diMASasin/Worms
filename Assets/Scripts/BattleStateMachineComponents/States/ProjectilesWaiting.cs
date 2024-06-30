using BattleStateMachineComponents.StatesData;
using CameraFollow;
using Configs;
using Pools;
using Projectiles;
using Timers;
using UnityEngine;

namespace BattleStateMachineComponents.States
{
    public class ProjectilesWaiting : IBattleState
    {
        private readonly IBattleStateSwitcher _battleStateSwitcher;
        private readonly Timer _timer;
        private readonly TimersConfig _timersConfig;
        private BattleStateMachineData _data;
        private IFollowingCamera _followingCamera;

        public ProjectilesWaiting(IBattleStateSwitcher battleStateSwitcher, Timer timer, BattleStateMachineData data)
        {
            _data = data;
            _timersConfig = _data.BattleConfig.TimersConfig;
            _battleStateSwitcher = battleStateSwitcher;
            _timer = timer;
        }

        public void Enter()
        {
            OnCountChanged(ProjectilePool.Count);
            
            ProjectilePool.CountChanged += OnCountChanged;
        }

        public void Exit() 
        {
            ProjectilePool.CountChanged -= OnCountChanged;
        }

        private void OnCountChanged(int count)
        {
            if(count == 0 && _timer.Started == false)
            {
                _timer.Start(_timersConfig.ProjectileWaitingDuration, () =>
                {
                    if(ProjectilePool.Count == 0)
                        _battleStateSwitcher.SwitchState<BetweenTurnsState>();
                });
            }
        }
    }
}