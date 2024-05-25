using Configs;
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
        private Timer TurnTimer => Data.TurnTimer;
        private TimersConfig GameConfigTimersConfig => Data.GameConfig.TimersConfig;
        
        private Team CurrentTeam
        {
            get => Data.CurrentTeam;
            set => Data.CurrentTeam = value;
        }

        private IWorm CurrentWorm
        {
            get => Data.CurrentWorm;
            set => Data.CurrentWorm = value;
        }

        public TurnState(IStateSwitcher stateSwitcher, BattleStateMachineData data) : base(stateSwitcher, data) { }


        public override void Enter()
        {
            CurrentTeam = Data.TeamsList.Next();
            CurrentWorm = Data.CurrentTeam.Worms.Next();
            
            CurrentWorm.SetCurrentWormLayer();
            CurrentWorm.SetRigidbodyDynamic();

            Data.Arrow.StartMove(CurrentWorm.Transform);
            Data.FollowingCamera.ZoomTarget();
            Data.FollowingCamera.SetTarget(CurrentWorm.Transform);
            Data.WeaponChanger.ChangeWorm(CurrentWorm);
            
            Data.PlayerInput.ChangeWorm(CurrentWorm);
            Data.PlayerInput.MovementInput.Enable(CurrentWorm.Movement);
            Data.PlayerInput.UIInput.Enable();
            
            Data.AllProjectileEvents.Launched += OnLaunched;
            
            TurnTimer.Start(GameConfigTimersConfig.TurnDuration, OnTimerElapsed);
        }

        public override void Exit()
        {
            TurnTimer.Stop();
            Data.WeaponSelector.Close();
            CurrentWorm.Movement.Reset();
            
            Data.PlayerInput.UIInput.Disable();
            Data.PlayerInput.WeaponInput.Disable();
            CoroutinePerformer.StartCoroutine(CurrentWorm.SetRigidbodyKinematicWhenGrounded());
            
            Data.AllProjectileEvents.Launched -= OnLaunched;
        }

        public override void Tick()
        {
        }

        public override void HandleInput()
        {
            Data.PlayerInput.MovementInput.Tick();
            Data.PlayerInput.WeaponInput.Tick();
        }

        private void OnLaunched(Projectile projectile, Vector2 velocity)
        {
            Data.FollowingCamera.SetTarget(projectile.transform);
            
            StateSwitcher.SwitchState<RetreatState>();
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