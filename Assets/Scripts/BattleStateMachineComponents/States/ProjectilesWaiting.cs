using System.Collections;
using CameraFollow;
using Configs;
using Infrastructure;
using Pools;
using UnityEngine;

namespace BattleStateMachineComponents.States
{
    public class ProjectilesWaiting : IBattleState
    {
        private readonly IBattleStateSwitcher _battleStateSwitcher;
        private readonly TimersConfig _timersConfig;
        private readonly ICoroutinePerformer _coroutinePerformer;
        private IFollowingCamera _followingCamera;

        public ProjectilesWaiting(IBattleStateSwitcher battleStateSwitcher, BattleStateMachineData data,
            ICoroutinePerformer coroutinePerformer)
        {
            _coroutinePerformer = coroutinePerformer;
            _timersConfig = data.BattleConfig.TimersConfig;
            _battleStateSwitcher = battleStateSwitcher;
        }

        public void Enter() => _coroutinePerformer.StartCoroutine(SwitchStateWhenNoProjectilesWithDelay());

        public void Exit() => _coroutinePerformer.StopCoroutine(SwitchStateWhenNoProjectilesWithDelay());

        private IEnumerator SwitchStateWhenNoProjectilesWithDelay()
        {
            while (ProjectilePool.Count > 0)
                yield return null;

            yield return new WaitForSeconds(_timersConfig.ProjectileWaitingDuration);

            if (ProjectilePool.Count == 0)
                _battleStateSwitcher.SwitchState<BetweenTurnsState>();
            else
                _coroutinePerformer.StartCoroutine(SwitchStateWhenNoProjectilesWithDelay());
        }
    }
}