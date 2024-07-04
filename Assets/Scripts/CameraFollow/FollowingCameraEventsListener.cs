using System;
using System.Collections;
using System.Collections.Generic;
using BattleStateMachineComponents.StatesData;
using EventProviders;
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
        private readonly IWeaponShotEvent _weaponShotEvent;
        private readonly ICoroutinePerformer _coroutinePerformer;
        private float _direction;
        private Coroutine _coroutine;
        private Coroutine _stopFollowWormCoroutine;
        private IWormEvents _wormEvents;

        public FollowingCameraEventsListener(IFollowingCamera followingCamera, IProjectileEvents projectileEvents,
            IExplosionEvents explosionEvents, IMovementInput movementInput, ICurrentWorm currentWorm,
            IWeaponShotEvent weaponShotEvent, ICoroutinePerformer coroutinePerformer, IWormEvents wormEvents)
        {
            _wormEvents = wormEvents;
            _coroutinePerformer = coroutinePerformer;
            _weaponShotEvent = weaponShotEvent;
            _currentWorm = currentWorm;
            _movementInput = movementInput;
            _followingCamera = followingCamera;
            _projectileEvents = projectileEvents;
            _explosionEvents = explosionEvents;

            _weaponShotEvent.WeaponShot += OnWeaponShot;
            _projectileEvents.Launched += OnLaunched;
            _projectileEvents.Exploded += OnProjectileExploded;
            _explosionEvents.Exploded += OnExploded;
            _explosionEvents.AnimationStopped += OnAnimationStopped;
            _movementInput.WalkPerformed += OnWalkPerformed;
            _wormEvents.DamageTook += OnDamageTook;
        }

        public void Dispose()
        {
            _weaponShotEvent.WeaponShot -= OnWeaponShot;
            _projectileEvents.Launched -= OnLaunched;
            _projectileEvents.Exploded -= OnProjectileExploded;
            _explosionEvents.Exploded -= OnExploded;
            _explosionEvents.AnimationStopped -= OnAnimationStopped;
            _movementInput.WalkPerformed -= OnWalkPerformed;
            _wormEvents.DamageTook -= OnDamageTook;
        }

        public void FollowWormIfMove()
        {
            if (_stopFollowWormCoroutine != null)
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
            // _followingCamera.RemoveTarget(projectile.transform);
            if (_stopFollowWormCoroutine != null)
                _coroutinePerformer.StopCoroutine(_stopFollowWormCoroutine);
        }

        private void OnAnimationStopped(Explosion explosion)
        {
            // _followingCamera.RemoveTarget(explosion.transform);
        }

        private void OnWeaponShot(float velocity, Weapon weapon)
        {
            // _followingCamera.RemoveAllTargets();
        }

        private void OnLaunched(Projectile projectile, Vector2 velocity)
        {
            if(_currentWorm.CurrentWorm != null)
                _stopFollowWormCoroutine = _coroutinePerformer.StartCoroutine(
                    StopFollowWhenFar(_currentWorm.CurrentWorm.transform, projectile.transform));

            _followingCamera.SetTarget(projectile.transform);
        }

        private void OnExploded(Explosion explosion) => _followingCamera.SetTarget(explosion.transform);

        private void OnWalkPerformed(float direction)
        {
            _direction = direction;
        }

        private IEnumerator StopFollowWhenFar(Transform target1, Transform target2)
        {
            int projectileWormDistance = 10;
            while (target1 != null && target2 != null && 
                   Vector3.Distance(target1.transform.position, target2.position) < projectileWormDistance)
                yield return null;

            _followingCamera.RemoveTarget(target1);
        }

        private IEnumerator StartListenMovementInRetreatState()
        {
            while (_direction == 0)
                yield return null;

            Debug.Log($"shish");
            _followingCamera.RemoveAllTargets();
            yield return null;
            _followingCamera.SetTarget(_currentWorm.CurrentWorm.transform);
        }
    }
}