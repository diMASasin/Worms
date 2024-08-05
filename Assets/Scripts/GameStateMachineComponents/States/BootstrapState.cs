using Infrastructure.Interfaces;
using InputService;
using UI_;
using Zenject;

namespace GameStateMachineComponents.States
{
    public class BootstrapState : IGameState
    {
        private readonly ISceneLoader _sceneLoader;
        private readonly LoadingScreen _loadingScreen;
        private readonly DiContainer _diContainer;
        private readonly MainMenu _mainMenu;
        private readonly IGameStateSwitcher _stateSwitcher;
        
        private MovementInput _movementInput;
        private CameraInput _cameraInput;
        private WeaponInput _weaponInput;
        private WeaponSelectorInput _weaponSelectorInput;

        public BootstrapState(DiContainer diContainer, IGameStateSwitcher stateSwitcher, ISceneLoader sceneLoader,
            LoadingScreen loadingScreen, MainMenu mainMenu)
        {
            _stateSwitcher = stateSwitcher;
            _mainMenu = mainMenu;
            _diContainer = diContainer;
            _loadingScreen = loadingScreen;
            _sceneLoader = sceneLoader;
        }
        
        public void Enter()
        {
            _diContainer.BindInterfacesAndSelfTo<SettingsWindow>().FromInstance(_mainMenu.SettingsWindow).AsSingle().NonLazy();

            _mainMenu.gameObject.SetActive(false);
            _loadingScreen.Init(_sceneLoader);
            _sceneLoader.Load(_sceneLoader.SceneNames.MainMenu, OnLoaded);
        }

        public void Exit()
        {
        }

        private void OnLoaded()
        {
            _stateSwitcher.SwitchState<MainMenuState>();
        }
    }
}