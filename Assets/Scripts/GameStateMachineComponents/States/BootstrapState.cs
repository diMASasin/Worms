using System.ComponentModel;
using Factories;
using Infrastructure;
using InputService;
using UI;
using UnityEngine;
using Weapons;
using Zenject;

namespace GameStateMachineComponents.States
{
    public class BootstrapState : GameState
    {
        private ISceneLoader _sceneLoader;
        private MovementInput _movementInput;
        private CameraInput _cameraInput;
        private WeaponInput _weaponInput;
        private WeaponSelectorInput _weaponSelectorInput;
        private LoadingScreen _loadingScreen;
        private DiContainer _diContainer;
        private MainMenu _mainMenu;

        public BootstrapState(DiContainer diContainer, IGameStateSwitcher stateSwitcher, ISceneLoader sceneLoader,
            LoadingScreen loadingScreen, MainMenu mainMenu) : base(stateSwitcher)
        {
            _mainMenu = mainMenu;
            _diContainer = diContainer;
            _loadingScreen = loadingScreen;
            _sceneLoader = sceneLoader;
        }
        
        public override void Enter()
        {
            _mainMenu.gameObject.SetActive(false);
            _loadingScreen.Init(_sceneLoader);
            _sceneLoader.Load(_sceneLoader.SceneNames.MainMenu, OnLoaded);
        }

        private void OnLoaded()
        {
            StateSwitcher.SwitchState<MainMenuState>();
        }

        public override void Exit()
        {
        }
    }
}