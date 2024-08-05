using Battle_;
using GameStateMachineComponents;
using GameStateMachineComponents.States;
using InputService;
using UI_;
using UnityEngine;
using Zenject;

namespace Infrastructure.Installers
{
    public class GlobalInstaller : MonoInstaller
    {
        [Header("Prefabs")]
        [SerializeField] private WeaponSelectorItem _weaponSelectorItemPrefab;
        [SerializeField] private FollowingTimerView _followingTimerViewPrefab;
        [SerializeField] private MainMenu _mainMenuPrefab;
        [SerializeField] private LoadingScreen _loadingScreenPrefab;
        
        private GameStateMachine _gameStateMachine;

        public override void InstallBindings()
        {
            BindInfrastructure();
            BindGameStateMachine();
            
            BindInput();
            BindPrefabs();
        }

        private void BindInfrastructure()
        {
            Container.BindInterfacesAndSelfTo<SceneLoader>().FromNew().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<BattleSettings>().FromNew().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<MainMenu>().FromComponentInNewPrefab(_mainMenuPrefab).AsSingle();
            Container.BindInterfacesAndSelfTo<LoadingScreen>().FromComponentInNewPrefab(_loadingScreenPrefab).AsSingle();
        }

        private void BindGameStateMachine()
        {
            Container.BindInterfacesAndSelfTo<BootstrapState>().FromNew().AsSingle();
            Container.BindInterfacesAndSelfTo<MainMenuState>().FromNew().AsSingle();
            Container.BindInterfacesAndSelfTo<LevelLoadState>().FromNew().AsSingle();
            Container.BindInterfacesAndSelfTo<GameLoopState>().FromNew().AsSingle();
            
            Container.BindInterfacesAndSelfTo<GameStateMachine>().FromNew().AsSingle();
        }

        private void BindInput()
        {
            var mainInput = new MainInput();
            
            Container.BindInterfacesAndSelfTo<CameraInput>().AsSingle();
            Container.BindInterfacesAndSelfTo<WeaponSelectorInput>().FromNew().AsSingle().WithArguments(mainInput.UI);
            Container.BindInterfacesAndSelfTo<WeaponInput>().AsSingle().WithArguments(mainInput.Weapon);
            Container.BindInterfacesAndSelfTo<MovementInput>().FromNew().AsSingle();
        }
        
        private void BindPrefabs()
        {
            Container.BindInstance(_weaponSelectorItemPrefab).AsSingle();
            Container.BindInstance(_followingTimerViewPrefab).AsSingle();
        }
    }
}