using System;
using System.Collections.Generic;
using Configs;
using MovementComponents;
using Projectiles.Behaviours.ExplodeBehaviour;
using Projectiles.Behaviours.LaunchBehaviour;
using UnityEngine;
using UnityEngine.Pool;
using WormComponents;

namespace Projectiles
{
    public class ProjectileConfigurator : IDisposable
    {
        private readonly Projectile _projectile;
        private readonly ProjectileConfig _config;
        private readonly ObjectPool<FollowingObject> _followingTimerViewPool;

        private readonly List<ILaunchBehaviour> _launchBehaviours = new();
        private readonly List<IExplodeBehaviour> _explodeBehaviours = new();
        private FollowingObject _followingObject;

        public ProjectileConfigurator(Projectile projectile, ProjectileConfig config,
            ObjectPool<FollowingObject> followingTimerViewPool)
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

        private void OnExploded(Projectile projectile)
        {
            foreach (var behaviour in _explodeBehaviours)
                behaviour.OnExplode();
            
            _followingObject.Disonnect();
            _followingTimerViewPool.Release(_followingObject);
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
                _launchBehaviours.Add(new OnLaunchTimer(config.ExplodeDelay, () => _projectile.Explode()));
                _followingObject = _followingTimerViewPool.Get();
                _followingObject.Connect(_projectile.transform);
            }
        }

        private SheepProjectile GetMovementBehaviour()
        {
            MovementConfig movementConfig = _config.MovementConfig;

            var groundChecker = new GroundChecker(_projectile.transform, _projectile.Collider, 
                movementConfig.GroundCheckerConfig);
            var sheepMovement = new SheepMovement(_projectile.Rigidbody, _projectile.Collider,
                _projectile.transform, groundChecker, movementConfig);

            SheepProjectile sheepProjectile = new SheepProjectile(sheepMovement);

            return sheepProjectile;
        }

        private ProjectileRotator GetRotator() => new (_config, _projectile.Rigidbody);

        private FragmentsExplodeBehaviour GetFragmentsBehaviour() => 
            new(_config.FragmentsPool, _config.FragmentsAmount, _projectile.transform);
    }
}