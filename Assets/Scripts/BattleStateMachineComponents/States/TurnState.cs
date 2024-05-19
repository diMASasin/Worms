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
        private const int RightButton = 1;
        
        private readonly Timer _timer = new();
        private Timer Timer => Data.TurnTimer;
        private Worm CurrentWorm => Data.CurrentWorm;
        private Arrow Arrow => Data.Arrow;
        private WeaponSelector WeaponSelector => Data.WeaponSelector;

        public TurnState(IStateSwitcher stateSwitcher, BattleStateMachineData data) : base(stateSwitcher, data) { }

        public override void Enter()
        {
            Data.CurrentTeam = Data.TeamsList.Next();
            Data.CurrentWorm = Data.WormsList.Next();
            // Debug.Log(Data.CurrentWorm.name + " " + Data.CurrentWorm.transform.position);
            
            CurrentWorm.SetCurrentWormLayer();
            CurrentWorm.SetRigidbodyDynamic();

            Arrow.StartMove(CurrentWorm.transform);
            Data.FollowingCamera.ZoomTarget();
            Data.FollowingCamera.SetTarget(CurrentWorm.transform);
            Data.PlayerInput.Enable(CurrentWorm);
            Data.WeaponChanger.ChangeWorm(CurrentWorm);
            
            Data.ProjectileLauncher.ProjectileLaunched += OnProjectileLaunched;
            Data.ProjectileLauncher.ProjectileExploded += OnProjectileExploded;
            
            Timer.Start(Data.TimersConfig.TurnDuration, OnTimerElapsed);
        }

        public override void Exit()
        {
            CurrentWorm.Movement.Reset();
            CoroutinePerformer.StartCoroutine(CurrentWorm.SetRigidbodyKinematicWhenGrounded());
            
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
            Data.PlayerInput.Tick();
            
            if (Input.GetMouseButtonDown(RightButton)) 
                WeaponSelector.Toggle();
        }

        private void OnProjectileLaunched(Projectile projectile, Vector2 velocity)
        {
            Data.WindMediator.InfluenceOnProjectileIfNecessary(projectile, projectile.Config);
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