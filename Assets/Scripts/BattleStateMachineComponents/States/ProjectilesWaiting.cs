using System.Threading;
using Cysharp.Threading.Tasks;
using Pools;
using static System.TimeSpan;
using static Cysharp.Threading.Tasks.UniTask;

namespace BattleStateMachineComponents.States
{
    public class ProjectilesWaiting : IBattleState
    {
        private readonly IBattleStateSwitcher _battleStateSwitcher;
        private readonly float _waitingDuration;
        private readonly IProjectilesCount _projectilesCount;
        private CancellationTokenSource _cancellationTokenSource;

        public ProjectilesWaiting(IBattleStateSwitcher battleStateSwitcher, BattleStateMachineData data, 
            IProjectilesCount projectilesCount)
        {
            _projectilesCount = projectilesCount;
            _waitingDuration = data.BattleConfig.TimersConfig.ProjectileWaitingDuration;
            _battleStateSwitcher = battleStateSwitcher;
        }

        public void Enter()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            SwitchStateWhenNoProjectilesWithDelay().Forget();
        }

        public void Exit()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }

        private async UniTaskVoid SwitchStateWhenNoProjectilesWithDelay()
        {
            await WaitUntil(() => _projectilesCount.Count <= 0, cancellationToken: _cancellationTokenSource.Token);

            await Delay(FromSeconds(_waitingDuration), cancellationToken: _cancellationTokenSource.Token);

            if (_projectilesCount.Count == 0)
                _battleStateSwitcher.SwitchState<BetweenTurnsState>();
            else
                SwitchStateWhenNoProjectilesWithDelay().Forget();
        }
    }
}