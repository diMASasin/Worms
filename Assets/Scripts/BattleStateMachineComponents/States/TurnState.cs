using BattleStateMachineComponents.StatesData;
using Configs;
using Infrastructure;
using Projectiles;
using Timers;
using UI;
using UnityEngine;
using Weapons;
using WormComponents;

namespace BattleStateMachineComponents.States
{
    public class TurnState : IBattleState
    {
        private readonly IStateSwitcher _stateSwitcher;
        private readonly GlobalBattleData _data;
        private readonly TurnStateData _turnStateData;
        private TimersConfig TimersConfig => _data.TimersConfig;
        private Timer GlobalTimer => _data.GlobalTimer;
        private Timer TurnTimer => _data.TurnTimer;

        private Team CurrentTeam { set => _data.CurrentTeam = value; }

        private Worm CurrentWorm
        {
            get => _data.CurrentWorm;
            set => _data.CurrentWorm = value;
        }

        public TurnState(IStateSwitcher stateSwitcher, GlobalBattleData data, TurnStateData turnStateData)
        {
            _stateSwitcher = stateSwitcher;
            _data = data;
            _turnStateData = turnStateData;
        }

        public void Enter()
        {
            CurrentTeam = _data.AliveTeams.Next();
            CurrentWorm = _data.CurrentTeam.Worms.Next();
            
            CurrentWorm.DelegateInput(_data.MovementInput);
            _turnStateData.WeaponSelector.AllowOpen();

            _turnStateData.Arrow.StartMove(CurrentWorm.Transform);
            _data.FollowingCamera.SetTarget(CurrentWorm.Transform);
            
            GlobalTimer.Resume();
            TurnTimer.Start(TimersConfig.TurnDuration, OnTimerElapsed);
            
            _turnStateData.WormEvents.WormDied += OnWormDied;
            _turnStateData.AllProjectileEvents.Launched += OnLaunched;
            _turnStateData.WeaponShotEvent.WeaponShot += OnWeaponShot;
        }

        public void Exit()
        {
            TurnTimer.Stop();
            GlobalTimer.Pause();
            
            _turnStateData.WeaponSelector.DisallowOpen();
            _turnStateData.WeaponSelector.CloseIfOpened();
            _turnStateData.Arrow.Disable();
            
            CurrentWorm.RemoveInput();
            
            _turnStateData.AllProjectileEvents.Launched -= OnLaunched;
            _turnStateData.WormEvents.WormDied -= OnWormDied;
            _turnStateData.WeaponShotEvent.WeaponShot -= OnWeaponShot;
        }

        public void Tick()
        {
        }

        public void FixedTick()
        {
        }

        private void OnWormDied(Worm worm)
        {
            _stateSwitcher.SwitchState<ProjectilesWaiting>();
        }

        private void OnWeaponShot(float velocity, Weapon weapon)
        {
            _stateSwitcher.SwitchState<RetreatState>();
        }

        private void OnLaunched(Projectile projectile, Vector2 velocity)
        {
            _data.FollowingCamera.SetTarget(projectile.transform);
        }

        private void OnTimerElapsed()
        {
            _stateSwitcher.SwitchState<ProjectilesWaiting>();
        }
    }
}