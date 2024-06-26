using Configs;
using Pools;
using Timers;

namespace BattleStateMachineComponents.States
{
    public class ProjectilesWaiting : IBattleState
    {
        private readonly IBattleStateSwitcher _battleStateSwitcher;
        private readonly Timer _timer;
        private TimersConfig _timersConfig;

        public ProjectilesWaiting(IBattleStateSwitcher battleStateSwitcher, Timer timer, TimersConfig timersConfig)
        {
            _timersConfig = timersConfig;
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

        public void Tick()
        {
        }

        public void FixedTick()
        {
        }

        public void LateTick()
        {
        }

        public void HandleInput()
        {
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