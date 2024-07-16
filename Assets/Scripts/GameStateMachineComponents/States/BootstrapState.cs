using Infrastructure;
using InputService;
using _UI;
using Weapons;
using Zenject;

namespace GameStateMachineComponents.States
{
    public class BootstrapState : GameState
    {
        private readonly ISceneLoader _sceneLoader;
        private MovementInput _movementInput;
        private CameraInput _cameraInput;
        private WeaponInput _weaponInput;
        private WeaponSelectorInput _weaponSelectorInput;
        private readonly LoadingScreen _loadingScreen;
        private DiContainer _diContainer;
        private readonly MainMenu _mainMenu;

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
            _diContainer.BindInterfacesAndSelfTo<SettingsWindow>().FromInstance(_mainMenu.SettingsWindow).AsSingle().NonLazy();

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