using System;
using System.Collections;
using BattleStateMachineComponents.StatesData;
using EventProviders;
using Explosion_;
using Infrastructure;
using Pools;
using Projectiles;
using UltimateCC;
using UnityEngine;
using Weapons;
using WormComponents;

namespace CameraFollow
{
    public class FollowingCameraEventsListener : IDisposable
    {
        private readonly IFollowingCamera _followingCamera;
        private readonly IProjectileEvents _projectileEvents;
        private readonly IExplosionEvents _explosionEvents;
        private readonly IMovementInput _movementInput;
        private readonly ICurrentWorm _currentWorm;
        private readonly ICoroutinePerformer _coroutinePerformer;
        private readonly IWormEvents _wormEvents;
        private float _wormDirection;
        private Coroutine _coroutine;
        private Coroutine _stopFollowWormCoroutine;

        public FollowingCameraEventsListener(IFollowingCamera followingCamera, IProjectileEvents projectileEvents,
            IExplosionEvents explosionEvents, IMovementInput movementInput, ICurrentWorm currentWorm,
            ICoroutinePerformer coroutinePerformer, IWormEvents wormEvents)
        {
            _wormEvents = wormEvents;
            _coroutinePerformer = coroutinePerformer;
            _currentWorm = currentWorm;
            _movementInput = movementInput;
            _followingCamera = followingCamera;
            _projectileEvents = projectileEvents;
            _explosionEvents = explosionEvents;

            _projectileEvents.Launched += OnLaunched;
            _projectileEvents.Exploded += OnProjectileExploded;
            _explosionEvents.Exploded += OnExploded;
            _movementInput.WalkPerformed += OnWalkPerformed;
            _wormEvents.DamageTook += OnDamageTook;
        }

        public void Dispose()
        {
            _projectileEvents.Launched -= OnLaunched;
            _projectileEvents.Exploded -= OnProjectileExploded;
            _explosionEvents.Exploded -= OnExploded;
            _movementInput.WalkPerformed -= OnWalkPerformed;
            _wormEvents.DamageTook -= OnDamageTook;
        }

        public void FollowWormIfMove()
        {
            if (_stopFollowWormCoroutine != null && _wormDirection != 0)
                _coroutinePerformer.StopCoroutine(_stopFollowWormCoroutine);
            
            _coroutine = _coroutinePerformer.StartCoroutine(StartListenMovementInRetreatState());
        }

        public void StopListenMovement() => _coroutinePerformer.StopCoroutine(_coroutine);

        private void OnDamageTook(Worm worm)
        {
            _coroutinePerformer.StartCoroutine(StopFollowWhenFar(_currentWorm.CurrentWorm.transform, worm.transform));
            _followingCamera.SetTarget(worm.transform);
        }

        private void OnProjectileExploded(Projectile projectile)
        {
            if (_stopFollowWormCoroutine != null)
                _coroutinePerformer.StopCoroutine(_stopFollowWormCoroutine);
        }

        private void OnLaunched(Projectile projectile, Vector2 velocity)
        {
               if(_currentWorm.CurrentWorm != null)
               {
                   Transform currentWormTransform = _currentWorm.CurrentWorm.transform;
                   
                   _stopFollowWormCoroutine = 
                       _coroutinePerformer.StartCoroutine(StopFollowWhenFar(currentWormTransform, projectile.transform));
               }

               _followingCamera.SetTarget(projectile.transform);
        }

        private void OnExploded(Explosion explosion) => _followingCamera.SetTarget(explosion.transform);

        private void OnWalkPerformed(float direction)
        {
            _wormDirection = direction;
        }

        private IEnumerator StopFollowWhenFar(Transform target1, Transform target2)
        {
            float projectileWormDistance = 10f;
            float distance;
            
            do
            {
                distance = Vector3.Distance(target1.transform.position, target2.position);
                yield return null;
            }
            while (target1 != null && target2 != null && distance < projectileWormDistance);

            _followingCamera.RemoveTarget(target1);
        }

        private IEnumerator StartListenMovementInRetreatState()
        {
            while (_wormDirection == 0)
                yield return null;

            _followingCamera.RemoveAllTargets();
            yield return null;
            _followingCamera.SetTarget(_currentWorm.CurrentWorm.transform);
        }
    }
}