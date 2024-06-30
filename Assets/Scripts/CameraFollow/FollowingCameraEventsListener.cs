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

namespace CameraFollow
{
    public class FollowingCameraEventsListener : IDisposable
    {
        private readonly IFollowingCamera _followingCamera;
        private readonly IProjectileEvents _projectileEvents;
        private readonly IExplosionEvents _explosionEvents;
        private IMovementInput _movementInput;
        private ICurrentWorm _currentWorm;
        private IWeaponShotEvent _weaponShotEvent;
        private ICoroutinePerformer _coroutinePerformer;
        private float _direction;
        private Coroutine _coroutine;

        public FollowingCameraEventsListener(IFollowingCamera followingCamera, IProjectileEvents projectileEvents, 
            IExplosionEvents explosionEvents, IMovementInput movementInput, ICurrentWorm currentWorm, IWeaponShotEvent weaponShotEvent,
            ICoroutinePerformer coroutinePerformer)
        {
            _coroutinePerformer = coroutinePerformer;
            _weaponShotEvent = weaponShotEvent;
            _currentWorm = currentWorm;
            _movementInput = movementInput;
            _followingCamera = followingCamera;
            _projectileEvents = projectileEvents;
            _explosionEvents = explosionEvents;

            _weaponShotEvent.WeaponShot += OnWeaponShot;
            _projectileEvents.Launched += OnLaunched;
            _explosionEvents.Exploded += OnExploded;
            _movementInput.WalkPerformed += OnWalkPerformed;
        }

        public void Dispose()
        {
            _weaponShotEvent.WeaponShot -= OnWeaponShot;
            _projectileEvents.Launched -= OnLaunched;
            _explosionEvents.Exploded -= OnExploded;
            _movementInput.WalkPerformed -= OnWalkPerformed;
        }

        public void FollowWormIfMove() => 
            _coroutine = _coroutinePerformer.StartCoroutine(StartListenMovementInRetreatState());
        public void StopListenMovement() => _coroutinePerformer.StopCoroutine(_coroutine);

        private void OnWeaponShot(float velocity, Weapon weapon)
        {
            // _followingCamera.RemoveAllTargets();
        }

        private void OnLaunched(Projectile projectile, Vector2 velocity)
        {
            _coroutinePerformer.StartCoroutine(StopFollowWormWhenFar(projectile.transform));
            _followingCamera.SetTarget(projectile.transform);
        }

        private void OnExploded(Explosion explosion) => _followingCamera.SetTarget(explosion.transform);

        private void OnWalkPerformed(float direction)
        {
            _direction = direction;
        }

        private IEnumerator StopFollowWormWhenFar(Transform projectile)
        {
            while (Vector3.Distance(_currentWorm.CurrentWorm.transform.position, projectile.position) < 10)
                yield return null;

            _followingCamera.RemoveTarget(_currentWorm.CurrentWorm.transform);
        }

        private IEnumerator StartListenMovementInRetreatState()
        {
            while (_direction == 0)
                yield return null;

            Debug.Log($"shish");
            _followingCamera.RemoveAllTargets();
            _followingCamera.SetTarget(_currentWorm.CurrentWorm.transform);
        }
    }
}