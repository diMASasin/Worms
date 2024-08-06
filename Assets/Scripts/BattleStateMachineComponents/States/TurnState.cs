using _2D_Ultimate_Side_Scroller_Character_Controller.Scripts.Input_System;
using CameraFollow;
using Configs;
using EventProviders;
using Pools;
using Projectiles;
using Timers;
using UI_.Message;
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
        private IWeaponShotEvent _weaponShotEvent;
        private IFollowingCamera _followingCamera;
        private readonly FollowingCameraEventsListener _followingCameraEventsListener;
        private IMessageShower _messageShower;

        private TimersConfig TimersConfig => _data.BattleConfig.TimersConfig;
        private ReactiveTimer BattleTimer => _data.BattleTimer;
        private ReactiveTimer TurnTimer => _data.TurnTimer;

        private Team CurrentTeam { set => _data.CurrentTeam = value; }

        private Worm CurrentWorm
        {
            get => _data.CurrentWorm;
            set => _data.CurrentWorm = value;
        }
        
        [Inject]
        public void Construct(IBattleStateSwitcher battleStateSwitcher, BattleStateMachineData data, IMovementInput movementInput, 
            Arrow arrow, IWormEvents wormEvents, IProjectileEvents allProjectileEvents, IWeaponShotEvent weaponShotEvent,
            WeaponChanger weaponChanger, IFollowingCamera followingCamera, IExplosionEvents explosionEvents, IMessageShower messageShower)
        {
            _messageShower = messageShower;
            _followingCamera = followingCamera;
            _weaponShotEvent = weaponShotEvent;
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
            _data.UI.WeaponSelector.AllowOpen();

            _arrow.StartMove(CurrentWorm.Transform);
            
            _followingCamera.ResetZoom();
            _followingCamera.SetTarget(CurrentWorm.Transform);
            
            BattleTimer.Resume();
            TurnTimer.Start(TimersConfig.TurnDuration, OnTimerElapsed);
            _messageShower.AppearTurnStartedText();
            
            _wormEvents.WormDied += OnWormDied;
            _weaponShotEvent.WeaponShot += OnWeaponShot;
        }

        public void Exit()
        {
            TurnTimer.Stop();
            BattleTimer.Pause();
            
            _data.UI.WeaponSelector.DisallowOpen();
            _data.UI.WeaponSelector.CloseIfOpened();
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