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
        private IProjectileEvents _projectileEvents;
        private BattleStateMachineData _data;

        public ProjectilesWaiting(IBattleStateSwitcher battleStateSwitcher, Timer timer, 
            IProjectileEvents projectileEvents, BattleStateMachineData data)
        {
            _data = data;
            _projectileEvents = projectileEvents;
            _timersConfig = _data.TimersConfig;
            _battleStateSwitcher = battleStateSwitcher;
            _timer = timer;
        }

        public void Enter()
        {
            OnCountChanged(ProjectilePool.Count);
            
            ProjectilePool.CountChanged += OnCountChanged;
            _projectileEvents.Launched += OnLaunched;
        }

        public void Exit()
        {
            ProjectilePool.CountChanged -= OnCountChanged;
            _projectileEvents.Launched -= OnLaunched;
        }

        private void OnLaunched(Projectile projectile, Vector2 velocity)
        {
            _data.FollowingCamera.SetTarget(projectile.transform);
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