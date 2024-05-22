using Infrastructure;
using Projectiles;
using Timers;
using UI;
using UnityEngine;
using WormComponents;

namespace BattleStateMachineComponents.States
{
    public class TurnState : BattleState
    {
        private readonly Timer _timer = new();
        private Timer Timer => Data.TurnTimer;
        private Worm CurrentWorm => Data.CurrentWorm;
        private Arrow Arrow => Data.Arrow;
        private WeaponSelector WeaponSelector => Data.WeaponSelector;

        public TurnState(IStateSwitcher stateSwitcher, BattleStateMachineData data) : base(stateSwitcher, data) { }

        public override void Enter()
        {
            Data.CurrentTeam = Data.TeamsList.Next();
            Data.CurrentWorm = Data.CurrentTeam.Worms.Next();
            // Debug.Log(Data.CurrentWorm.name + " " + Data.CurrentWorm.transform.position);
            
            CurrentWorm.SetCurrentWormLayer();
            CurrentWorm.SetRigidbodyDynamic();

            Arrow.StartMove(CurrentWorm.transform);
            Data.FollowingCamera.ZoomTarget();
            Data.FollowingCamera.SetTarget(CurrentWorm.transform);
            Data.WeaponChanger.ChangeWorm(CurrentWorm);
            
            Data.PlayerInput.ChangeWorm(CurrentWorm);
            Data.PlayerInput.MovementInput.Enable(CurrentWorm.Movement);
            Data.PlayerInput.UIInput.Enable();
            
            Data.ProjectileLauncher.ProjectileLaunched += OnProjectileLaunched;
            Data.ProjectileLauncher.ProjectileExploded += OnProjectileExploded;
            
            Timer.Start(Data.TimersConfig.TurnDuration, OnTimerElapsed);
        }

        public override void Exit()
        {
            Data.TurnTimer.Stop();
            WeaponSelector.Close();
            CurrentWorm.Movement.Reset();
            
            Data.PlayerInput.UIInput.Disable();
            Data.PlayerInput.WeaponInput.Disable();
            CoroutinePerformer.StartCoroutine(CurrentWorm.SetRigidbodyKinematicWhenGrounded());
            
            // _timer.Start(CurrentWorm.Config.RemoveWeaponDelay, () => CurrentWorm.RemoveWeapon());
            
            Data.ProjectileLauncher.ProjectileLaunched -= OnProjectileLaunched;
            Data.ProjectileLauncher.ProjectileExploded -= OnProjectileExploded;
        }

        public override void Tick()
        {
        }

        public override void HandleInput()
        {
            Data.PlayerInput.MovementInput.Tick();
            Data.PlayerInput.WeaponInput.Tick();
        }

        private void OnProjectileLaunched(Projectile projectile, Vector2 velocity)
        {
            Data.WindMediator.InfluenceOnProjectileIfNecessary(projectile, projectile.Config);
            Data.FollowingCamera.SetTarget(projectile.transform);
            
            StateSwitcher.SwitchState<RetreatState>();
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