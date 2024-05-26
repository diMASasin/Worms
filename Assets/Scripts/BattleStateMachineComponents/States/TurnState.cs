using BattleStateMachineComponents.StatesData;
using Configs;
using Infrastructure;
using Projectiles;
using Timers;
using UI;
using UnityEngine;
using WormComponents;

namespace BattleStateMachineComponents.States
{
    public class TurnState : IBattleState
    {
        private readonly IStateSwitcher _stateSwitcher;
        private readonly BattleStateMachineData _data;
        private readonly TurnStateData _turnStateData;
        private Timer TurnTimer => _data.TurnTimer;
        private TimersConfig TimersConfig => _data.TimersConfig;
        
        private Team CurrentTeam { set => _data.CurrentTeam = value; }

        private IWorm CurrentWorm
        {
            get => _data.CurrentWorm;
            set => _data.CurrentWorm = value;
        }

        public TurnState(IStateSwitcher stateSwitcher, BattleStateMachineData data, TurnStateData turnStateData)
        {
            _stateSwitcher = stateSwitcher;
            _data = data;
            _turnStateData = turnStateData;
        }
        
        public void Enter()
        {
            CurrentTeam = _turnStateData.AliveTeams.Next();
            CurrentWorm = _data.CurrentTeam.Worms.Next();
            
            CurrentWorm.SetCurrentWormLayer();
            CurrentWorm.SetRigidbodyDynamic();

            _turnStateData.WeaponChanger.ChangeWorm(CurrentWorm);
            _turnStateData.Arrow.StartMove(CurrentWorm.Transform);
            _data.FollowingCamera.ZoomTarget();
            _data.FollowingCamera.SetTarget(CurrentWorm.Transform);
            
            _data.PlayerInput.ChangeWorm(CurrentWorm);
            _data.PlayerInput.MovementInput.Enable(CurrentWorm.Movement);
            _data.PlayerInput.UIInput.Enable();
            
            _turnStateData.AllProjectileEvents.Launched += OnLaunched;
            
            TurnTimer.Start(TimersConfig.TurnDuration, OnTimerElapsed);
        }

        public void Exit()
        {
            TurnTimer.Stop();
            _turnStateData.WeaponSelector.Close();
            CurrentWorm.Movement.Reset();
            
            _data.PlayerInput.UIInput.Disable();
            _data.PlayerInput.WeaponInput.Disable();
            CoroutinePerformer.StartCoroutine(CurrentWorm.SetRigidbodyKinematicWhenGrounded());
            
            _turnStateData.AllProjectileEvents.Launched -= OnLaunched;
        }

        public void Tick()
        {
        }

        public void HandleInput()
        {
            _data.PlayerInput.MovementInput.Tick();
            _data.PlayerInput.WeaponInput.Tick();
        }

        private void OnLaunched(Projectile projectile, Vector2 velocity)
        {
            _data.FollowingCamera.SetTarget(projectile.transform);
            
            _stateSwitcher.SwitchState<RetreatState>();
        }

        private void OnTimerElapsed()
        {
            if (TryFinishShot()) return;
            
            _stateSwitcher.SwitchState<BetweenTurnsState>();
        }

        private bool TryFinishShot()
        {
            if (CurrentWorm.Weapon?.CurrentShotPower > 0)
            {
                CurrentWorm.Weapon.Shoot();
                _stateSwitcher.SwitchState<ProjectilesWaiting>();
                return true;
            }

            return false;
        }
    }
}