using CameraFollow;
using Configs;
using EventProviders;
using Pools;
using Projectiles;
using Timers;
using UltimateCC;
using Weapons;
using WormComponents;
using Zenject;

namespace BattleStateMachineComponents.States
{
    public class TurnState : IBattleState
    {
        private IBattleStateSwitcher _battleStateSwitcher;
        private BattleStateMachineData _data;
        private IMovementInput _movementInput;
        private Arrow _arrow;
        private IWormEvents _wormEvents;
        private IProjectileEvents _allProjectileEvents;
        private IWeaponShotEvent _weaponShotEvent;
        private WeaponChanger _weaponChanger;
        private IFollowingCamera _followingCamera;
        private IExplosionEvents _explosionEvents;
        private readonly FollowingCameraEventsListener _followingCameraEventsListener;

        private TimersConfig TimersConfig => _data.BattleConfig.TimersConfig;
        private Timer BattleTimer => _data.BattleTimer;
        private Timer TurnTimer => _data.TurnTimer;

        private Team CurrentTeam { set => _data.CurrentTeam = value; }

        private Worm CurrentWorm
        {
            get => _data.CurrentWorm;
            set => _data.CurrentWorm = value;
        }
        
        [Inject]
        public void Construct(IBattleStateSwitcher battleStateSwitcher, BattleStateMachineData data, IMovementInput movementInput, 
            Arrow arrow, IWormEvents wormEvents, IProjectileEvents allProjectileEvents, IWeaponShotEvent weaponShotEvent,
            WeaponChanger weaponChanger, IFollowingCamera followingCamera, IExplosionEvents explosionEvents)
        {
            _explosionEvents = explosionEvents;
            _followingCamera = followingCamera;
            _weaponChanger = weaponChanger;
            _weaponShotEvent = weaponShotEvent;
            _allProjectileEvents = allProjectileEvents;
            _wormEvents = wormEvents;
            _arrow = arrow;
            _movementInput = movementInput;
            _battleStateSwitcher = battleStateSwitcher;
            _data = data;
        }

        public void Enter()
        {
            CurrentTeam = _data.AliveTeams.Next();
            CurrentWorm = _data.CurrentTeam.Worms.Next();
            
            CurrentWorm.DelegateInput(_movementInput);
            _data.WeaponSelector.AllowOpen();

            _arrow.StartMove(CurrentWorm.Transform);
            _followingCamera.ResetZoom();
            _followingCamera.RemoveAllTargets();
            _followingCamera.SetTarget(CurrentWorm.Transform);
            
            BattleTimer.Resume();
            TurnTimer.Start(TimersConfig.TurnDuration, OnTimerElapsed);
            
            _wormEvents.WormDied += OnWormDied;
            _weaponShotEvent.WeaponShot += OnWeaponShot;
        }

        public void Exit()
        {
            TurnTimer.Stop();
            BattleTimer.Pause();
            
            _data.WeaponSelector.DisallowOpen();
            _data.WeaponSelector.CloseIfOpened();
            _arrow.Disable();
            
            CurrentWorm.RemoveInput();
            
            _wormEvents.WormDied -= OnWormDied;
            _weaponShotEvent.WeaponShot -= OnWeaponShot;
        }

        private void OnWormDied(Worm worm) => _battleStateSwitcher.SwitchState<ProjectilesWaiting>();

        private void OnWeaponShot(float velocity, Weapon weapon) => _battleStateSwitcher.SwitchState<RetreatState>();

        private void OnTimerElapsed() => _battleStateSwitcher.SwitchState<ProjectilesWaiting>();
    }
}