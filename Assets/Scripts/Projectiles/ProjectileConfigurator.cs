using System;
using System.Collections.Generic;
using Configs;
using MovementComponents;
using Pools;
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
        private readonly IPool<FollowingTimerView> _pool;

        private readonly List<ILaunchBehaviour> _launchBehaviours = new();
        private readonly List<IExplodeBehaviour> _explodeBehaviours = new();
        private SheepMovement _sheepMovement;
        private GroundChecker _groundChecker;

        public ProjectileConfigurator(IPool<FollowingTimerView> pool)
        {
            _pool = pool;
        }

        public void Dispose()
        {
            
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
            projectile.Exploded -= OnExploded;
            
            foreach (var behaviour in _explodeBehaviours)
                behaviour.OnExplode();

            _sheepMovement?.JumpTimer.Stop();
        }

        private void OnLaunched(Projectile projectile, Vector2 shotPower)
        {
            projectile.Launched -= OnLaunched;
            
            foreach (var behaviour in _launchBehaviours)
                behaviour.OnLaunch(shotPower);
        }
        
        public void Configure(Projectile projectile, ProjectileConfig config)
        {
            projectile.Exploded += OnExploded;
            projectile.Launched += OnLaunched;
            
            if (config.CanWalk == true)
                _launchBehaviours.Add(GetMovementBehaviour(projectile, config.MovementConfig));

            if (config.HasFragments == true)
                _explodeBehaviours.Add(GetFragmentsBehaviour(config, projectile.transform));

            if (config.TorqueRange.StartValue != 0 || config.TorqueRange.EndValue != 0)
                _launchBehaviours.Add(GetRotator(config, projectile.Rigidbody));

            if (config.ExplodeOnKeyDown)
                _launchBehaviours.Add(new ExplodeOnKeyDown(projectile));
            
            if (config.IsExplodeWithDelay)
            {
                var onLaunchTimer = new OnLaunchTimer(_pool, projectile,
                    config.ExplodeDelay, () => projectile.Explode());
                _launchBehaviours.Add(onLaunchTimer);
            }
        }

        private SheepProjectile GetMovementBehaviour(Projectile projectile, MovementConfig movementConfig)
        {
            _groundChecker = new GroundChecker(projectile.transform, projectile.Collider, 
                movementConfig.GroundCheckerConfig);
            _sheepMovement = new SheepMovement(projectile.Rigidbody, projectile.Collider,
                projectile.transform, _groundChecker, movementConfig);

            projectile.Rigidbody.freezeRotation = true;
            SheepProjectile sheepProjectile = new SheepProjectile(_sheepMovement);

            return sheepProjectile;
        }

        private ProjectileRotator GetRotator(ProjectileConfig config, Rigidbody2D rigidbody) => new (config, rigidbody);

        private FragmentsExplodeBehaviour GetFragmentsBehaviour(ProjectileConfig config, Transform projectileTransform) => 
            new(config.FragmentsPool, config.FragmentsAmount, projectileTransform);
    }
}