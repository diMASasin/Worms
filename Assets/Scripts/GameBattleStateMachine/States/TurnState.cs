using System.Collections.Generic;
using Projectiles;
using Timers;
using UnityEngine;
using WormComponents;

namespace GameBattleStateMachine.States
{
    public class TurnState : BattleState
    {
        private readonly Timer _timer = new();
        private const int LeftButton = 1;
        private Timer Timer => Data.TurnTimer;
        private Worm CurrentWorm => Data.CurrentWorm;
        private Arrow Arrow => Data.Arrow;
        private WeaponSelector WeaponSelector => Data.WeaponSelector;

        public TurnState(IStateSwitcher stateSwitcher, BattleStateMachineData data) : base(stateSwitcher, data) { }

        public override void Enter()
        {
            Data.TryGetNextTeam(out Team team);
            team.TryGetNextWorm(out Worm worm);
            worm.SetCurrentWormLayer();
            
            Data.CurrentTeam = team;
            Data.CurrentWorm = worm;

            Arrow.StartMove(worm.transform);
            Data.FollowingCamera.ZoomTarget();
            Data.FollowingCamera.SetTarget(worm.transform);
            Data.PlayerInput.Enable(worm);
            Data.WeaponChanger.ChangeWorm(CurrentWorm);
            WeaponSelector.Enable();
            
            Data.ProjectileLauncher.ProjectileLaunched += OnProjectileLaunched;
            Data.ProjectileLauncher.ProjectileExploded += OnProjectileExploded;
            
            Timer.Start(Data.TimersConfig.TurnDuration, OnTimerElapsed);
        }

        public override void Exit()
        {
            WeaponSelector.Disable();
            Data.PlayerInput.Disable();
            CurrentWorm.Movement.Reset();
            
            _timer.Start(CurrentWorm.Config.RemoveWeaponDelay, () => CurrentWorm.RemoveWeapon());
            
            Data.ProjectileLauncher.ProjectileLaunched -= OnProjectileLaunched;
            Data.ProjectileLauncher.ProjectileExploded -= OnProjectileExploded;
        }

        public override void Tick()
        {
            Arrow.Tick();
        }

        public override void HandleInput()
        {
            if (Input.GetMouseButtonDown(LeftButton)) 
                Data.WeaponSelector.ToggleIfAllowed();
        }

        private void OnProjectileLaunched(Projectile projectile, Vector2 velocity)
        {
            Data.WindMediator.InfluenceOnProjectileIfNecessary(projectile, projectile.Config);
        }

        private void OnProjectileExploded(Projectile projectile)
        {
            Data.WindMediator.RemoveProjectileFromInfluence(projectile);
        }

        private void OnTimerElapsed()
        {
            if (TryFinishShot()) return;
            
            StateSwitcher.SwitchState<BetweenTurnsState>();
        }

        private bool TryFinishShot()
        {
            if (CurrentWorm.Weapon?.CurrentShotPower > 0)
            {
                CurrentWorm.Weapon.Shoot();
                StateSwitcher.SwitchState<ProjectilesWaiting>();
                return true;
            }

            return false;
        }
    }
}