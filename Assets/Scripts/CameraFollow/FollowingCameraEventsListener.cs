using System;
using System.Threading;
using _2D_Ultimate_Side_Scroller_Character_Controller.Scripts.Input_System;
using BattleStateMachineComponents.StatesData;
using Cysharp.Threading.Tasks;
using EventProviders;
using Explosion_;
using Pools;
using Projectiles;
using UnityEngine;
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
        private readonly IWormEvents _wormEvents;
        
        private CancellationTokenSource _stopListenMovementSource;
        private CancellationTokenSource _stopFollowWormSource;
        
        private float _wormDirection;

        public FollowingCameraEventsListener(IFollowingCamera followingCamera, IProjectileEvents projectileEvents,
            IExplosionEvents explosionEvents, IMovementInput movementInput, ICurrentWorm currentWorm,
            IWormEvents wormEvents)
        {
            _wormEvents = wormEvents;
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
            _wormEvents.WormDied += OnWormDied;
        }

        public void Dispose()
        {
            _projectileEvents.Launched -= OnLaunched;
            _projectileEvents.Exploded -= OnProjectileExploded;
            _explosionEvents.Exploded -= OnExploded;
            _movementInput.WalkPerformed -= OnWalkPerformed;
            _wormEvents.DamageTook -= OnDamageTook;
            _wormEvents.WormDied -= OnWormDied;
        }

        public void FollowWormIfMove()
        {
            if (_wormDirection != 0)
                _stopFollowWormSource.Cancel();

            _stopListenMovementSource = new CancellationTokenSource();
            StartListenMovementInRetreatState().Forget();
        }

        public void StopListenMovement() => _stopListenMovementSource.Cancel();

        private void OnDamageTook(Worm worm)
        {
            StopFollowWhenFar(_currentWorm.CurrentWorm.transform, worm.transform).Forget();
            _followingCamera.SetTarget(worm.transform);
        }

        private void OnProjectileExploded(Projectile projectile) => _stopFollowWormSource.Cancel();

        private void OnLaunched(Projectile projectile, Vector2 velocity)
        {
               if (_currentWorm.CurrentWorm != null)
               {
                   Transform currentWormTransform = _currentWorm.CurrentWorm.transform;
                   
                   StopFollowWhenFar(currentWormTransform, projectile.transform).Forget();
               }

               _followingCamera.SetTarget(projectile.transform);
        }

        private void OnExploded(Explosion explosion) => _followingCamera.SetTarget(explosion.transform);

        private void OnWalkPerformed(float direction)
        {
            _wormDirection = direction;
        }

        private async UniTaskVoid StopFollowWhenFar(Transform target1, Transform target2)
        {
            _stopFollowWormSource = new CancellationTokenSource();
            float projectileWormDistance = 10f;
            float distance;
            
            do
            {
                if(_stopFollowWormSource.IsCancellationRequested == true)
                    return;
                
                distance = Vector3.Distance(target1.transform.position, target2.position);
                await UniTask.Yield();
            }
            while (target1 != null && target2 != null && distance < projectileWormDistance);

            _followingCamera.RemoveTarget(target1);
        }

        private async UniTaskVoid StartListenMovementInRetreatState()
        {
            await UniTask.WaitUntil(() => _wormDirection != 0, cancellationToken: _stopListenMovementSource.Token);

            _followingCamera.RemoveAllTargets();
            
            await UniTask.WaitUntil(() => _followingCamera.HasTarget == false, cancellationToken: _stopListenMovementSource.Token);
            
            _followingCamera.SetTarget(_currentWorm.CurrentWorm.transform);
        }

        private void OnWormDied(Worm worm)
        {
            _stopFollowWormSource.Cancel();        
            _stopListenMovementSource.Cancel();
        }
    }
}