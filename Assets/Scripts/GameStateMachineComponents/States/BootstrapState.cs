using System;
using Battle_;
using CameraFollow;
using Infrastructure;
using InputService;
using Services;
using UI;
using UltimateCC;
using UnityEngine;
using Weapons;
using Object = UnityEngine.Object;

namespace GameStateMachineComponents.States
{
    public class BootstrapState : GameState, IDisposable
    {
        private readonly ISceneLoader _sceneLoader;
        private MovementMovementInput _movementMovementInput;
        private CameraInput _cameraInput;
        private WeaponInput _weaponInput;
        private WeaponSelectorInput _weaponSelectorInput;

        public BootstrapState(GameStateMachineData data, IGameStateSwitcher stateSwitcher, AllServices services) : 
            base(data, stateSwitcher)
        {
            RegisterServices(services);
            
            _sceneLoader = services.Single<ISceneLoader>();
        }

        public void Dispose()
        {
            _movementMovementInput.Unsubscribe();
            _weaponInput.Unsubscribe();
            _weaponSelectorInput.Unsubscribe();
        }

        public override void Enter()
        {            
            Data.LoadingScreen.Init(_sceneLoader);
            
            _sceneLoader.Load(_sceneLoader.SceneNames.MainMenu, OnLoaded);
        }

        private void OnLoaded()
        {
            StateSwitcher.SwitchState<MainMenuState>();
        }

        public override void Exit()
        {
        }

        private void RegisterServices(AllServices services)
        {
            CoroutinePerformer coroutinePerformer = Object.Instantiate(Data.CoroutinePerformerPrefab, Data.GameParent);
            services.RegisterSingle<ICoroutinePerformer>(coroutinePerformer);
            services.RegisterSingle<ISceneLoader>(new SceneLoader(coroutinePerformer));
            services.RegisterSingle<IBattleSettings>(new BattleSettings());
            
            RegisterInput(services);
        }

        private void RegisterInput(AllServices services)
        {
            var mainInput = new MainInput();
            
            _movementMovementInput = new MovementMovementInput();
            _cameraInput = new CameraInput();
            _weaponInput = new WeaponInput(mainInput.Weapon);
            _weaponSelectorInput = new WeaponSelectorInput(mainInput.UI);
            
            services.RegisterSingle<ICameraInput>(_cameraInput);
            services.RegisterSingle<IWeaponInput>(_weaponInput);
            services.RegisterSingle<IWeaponSelectorInput>(_weaponSelectorInput);
            services.RegisterSingle<IMovementInput>(_movementMovementInput);
            
            _weaponInput.Subscribe();
            _weaponSelectorInput.Subscribe();
            _movementMovementInput.Subscribe();
        }
    }
}