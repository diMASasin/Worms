using System;
using System.Collections.Generic;
using Configs;
using MovementComponents;
using Projectiles.Behaviours.ExplodeBehaviour;
using Projectiles.Behaviours.LaunchBehaviour;
using Timers;
using UI;
using UnityEngine;
using UnityEngine.Pool;
using WormComponents;

namespace Projectiles
{
    public class ProjectileConfigurator : IDisposable
    {
        private readonly Projectile _projectile;
        private readonly ProjectileConfig _config;
        private readonly ObjectPool<FollowingTimerView> _followingTimerViewPool;

        private readonly List<ILaunchBehaviour> _launchBehaviours = new();
        private readonly List<IExplodeBehaviour> _explodeBehaviours = new();
        private FollowingTimerView _followingTimerView;
        private SheepMovement _sheepMovement;
        private GroundChecker _groundChecker;

        public ProjectileConfigurator(Projectile projectile, ProjectileConfig config,
            ObjectPool<FollowingTimerView> followingTimerViewPool)
        {
            _config = config;
            _followingTimerViewPool = followingTimerViewPool;
            _projectile = projectile;

            _projectile.Exploded += OnExploded;
            _projectile.Launched += OnLaunched;
        }

        public void Dispose()
        {
            _projectile.Exploded -= OnExploded;
            _projectile.Launched -= OnLaunched;
        }

        public void FixedTick()
        {
            _sheepMovement?.FixedTick();
            _groundChecker?.FixedTick();
        }

        public void OnDrawGizmos()
        {
            _sheepMovement?.OnDrawGizmos();
            _groundChecker?.OnDrawGizmos();
        }

        private void OnExploded(Projectile projectile)
        {
            foreach (var behaviour in _explodeBehaviours)
                behaviour.OnExplode();

            _sheepMovement?.JumpTimer.Stop();
        }

        private void OnLaunched(Projectile projectile, Vector2 shotPower)
        {
            foreach (var behaviour in _launchBehaviours)
                behaviour.OnLaunch(shotPower);
        }
        
        public void Configure(ProjectileConfig config)
        {
            if (config.CanWalk == true)
                _launchBehaviours.Add(GetMovementBehaviour());

            if (config.HasFragments == true)
                _explodeBehaviours.Add(GetFragmentsBehaviour());

            if (config.TorqueRange.StartValue != 0 || config.TorqueRange.EndValue != 0)
                _launchBehaviours.Add(GetRotator());

            if (config.ExplodeOnKeyDown)
                _launchBehaviours.Add(new ExplodeOnKeyDown(_projectile));
            
            if (config.IsExplodeWithDelay)
            {
                var onLaunchTimer = new OnLaunchTimer(_followingTimerViewPool, _projectile,
                    config.ExplodeDelay, () => _projectile.Explode());
                _launchBehaviours.Add(onLaunchTimer);
            }
        }

        private SheepProjectile GetMovementBehaviour()
        {
            MovementConfig movementConfig = _config.MovementConfig;

            _groundChecker = new GroundChecker(_projectile.transform, _projectile.Collider, 
                movementConfig.GroundCheckerConfig);
            _sheepMovement = new SheepMovement(_projectile.Rigidbody, _projectile.Collider,
                _projectile.transform, _groundChecker, movementConfig);

            _projectile.Rigidbody.freezeRotation = true;
            SheepProjectile sheepProjectile = new SheepProjectile(_sheepMovement);

            return sheepProjectile;
        }

        private ProjectileRotator GetRotator() => new (_config, _projectile.Rigidbody);

        private FragmentsExplodeBehaviour GetFragmentsBehaviour() => 
            new(_config.FragmentsPool, _config.FragmentsAmount, _projectile.transform);
    }
}