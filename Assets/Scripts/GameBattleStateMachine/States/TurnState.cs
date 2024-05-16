using System.Collections.Generic;
using Projectiles;
using Timers;
using UnityEngine;

namespace GameBattleStateMachine.States
{
    public class TurnState : IBattleState
    {
        private readonly IStateSwitcher _stateSwitcher;
        private readonly BattleStateMachineData _data;

        private Timer Timer => _data.TurnTimer;
        private int CurrentTeamIndex => _data.CurrentTeamIndex;
        private List<Team> AliveTeams => _data.AliveTeams;
        private Worm CurrentWorm => _data.CurrentWorm;
        private Arrow Arrow => _data.Arrow;
        private WeaponSelector WeaponSelector => _data.WeaponSelector;

        public TurnState(IStateSwitcher stateSwitcher, BattleStateMachineData data)
        {
            _stateSwitcher = stateSwitcher;
            _data = data;
        }

        public void Enter()
        {
            _data.TryGetNextTeam(out Team team);
            team.TryGetNextWorm(out Worm worm);
            worm.SetCurrentWormLayer();
            
            _data.CurrentTeam = team;
            _data.CurrentWorm = worm;

            Arrow.StartMove(worm.transform);
            _data.FollowingCamera.ZoomTarget();
            _data.FollowingCamera.SetTarget(worm.transform);
            _data.Input.Enable(worm);
            _data.WeaponChanger.ChangeWorm(CurrentWorm);
            WeaponSelector.Enable();
            
            _data.ProjectileLauncher.ProjectileLaunched += OnProjectileLaunched;
            
            Timer.Start(_data.TimersConfig.TurnDuration, OnTimerElapsed);
        }

        public void Exit()
        {
            WeaponSelector.Disable();
            _data.Input.Disable();
            CurrentWorm.Movement.Reset();
            
            Timer timer = new Timer();
            timer.Start(CurrentWorm.Config.RemoveWeaponDelay, () => CurrentWorm.RemoveWeapon());
            
            _data.ProjectileLauncher.ProjectileLaunched -= OnProjectileLaunched;
        }

        public void Tick()
        {
            Arrow.Tick();
        }

        private void OnProjectileLaunched(Projectile projectile, Vector2 velocity)
        {
            
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