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
        private Timer TurnTimer => _data.GlobalBattleData.TurnTimer;
        private TimersConfig TimersConfig => _data.GlobalBattleData.TimersConfig;
        
        private Team CurrentTeam { set => _data.GlobalBattleData.CurrentTeam = value; }

        private IWorm CurrentWorm
        {
            get => _data.GlobalBattleData.CurrentWorm;
            set => _data.GlobalBattleData.CurrentWorm = value;
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
            CurrentWorm = _data.GlobalBattleData.CurrentTeam.Worms.Next();
            
            CurrentWorm.SetCurrentWormLayer();
            CurrentWorm.SetRigidbodyDynamic();

            _turnStateData.WeaponChanger.ChangeWorm(CurrentWorm);
            _turnStateData.Arrow.StartMove(CurrentWorm.Transform);
            _data.GlobalBattleData.FollowingCamera.ZoomTarget();
            _data.GlobalBattleData.FollowingCamera.SetTarget(CurrentWorm.Transform);
            
            _data.GlobalBattleData.PlayerInput.ChangeWorm(CurrentWorm);
            _data.GlobalBattleData.PlayerInput.MovementInput.Enable(CurrentWorm.Movement);
            _data.GlobalBattleData.PlayerInput.UIInput.Enable();
            
            _turnStateData.AllProjectileEvents.Launched += OnLaunched;
            
            TurnTimer.Start(TimersConfig.TurnDuration, OnTimerElapsed);
        }

        public void Exit()
        {
            TurnTimer.Stop();
            _turnStateData.WeaponSelector.Close();
            CurrentWorm.Movement.Reset();
            
            _data.GlobalBattleData.PlayerInput.UIInput.Disable();
            _data.GlobalBattleData.PlayerInput.WeaponInput.Disable();
            CoroutinePerformer.StartCoroutine(CurrentWorm.SetRigidbodyKinematicWhenGrounded());
            
            _turnStateData.AllProjectileEvents.Launched -= OnLaunched;
        }

        public void Tick()
        {
        }

        public void HandleInput()
        {
            _data.GlobalBattleData.PlayerInput.MovementInput.Tick();
            _data.GlobalBattleData.PlayerInput.WeaponInput.Tick();
        }

        private void OnLaunched(Projectile projectile, Vector2 velocity)
        {
            _data.GlobalBattleData.FollowingCamera.SetTarget(projectile.transform);
            
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