using Battle_;
using GameStateMachineComponents;
using InputService;
using UI;
using UnityEngine;
using Water;
using Weapons;
using Zenject;

namespace Infrastructure.Installers
{
    public class GlobalInstaller : MonoInstaller
    {
        [SerializeField] private CoroutinePerformer _coroutinePerformer;
        
        [Header("Prefabs")]
        [SerializeField] private WeaponSelectorItem _weaponSelectorItemPrefab;
        [SerializeField] private FollowingTimerView _followingTimerViewPrefab;
        [SerializeField] private MainMenu _mainMenuPrefab;
        [SerializeField] private LoadingScreen _loadingScreenPrefab;
        [SerializeField] private Material _stylizedWaterMaterial;
        
        private GameStateMachine _gameStateMachine;

        public override void InstallBindings()
        {
            BindInfrastructure();
            BindGameStateMachine();
            BindInput();
            BindPrefabs();
            BindWaterMaterial();
        }

        private void BindInfrastructure()
        {
            Container.BindInterfacesAndSelfTo<CoroutinePerformer>().FromInstance(_coroutinePerformer).AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<SceneLoader>().FromNew().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<BattleSettings>().FromNew().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<MainMenu>().FromComponentInNewPrefab(_mainMenuPrefab).AsSingle();
            Container.BindInterfacesAndSelfTo<LoadingScreen>().FromComponentInNewPrefab(_loadingScreenPrefab).AsSingle();
        }

        private void BindGameStateMachine() => 
            Container.BindInterfacesAndSelfTo<GameStateMachine>().FromNew().AsSingle().NonLazy();

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

        private void BindWaterMaterial() => 
            Container.Bind<WaterVelocityChanger>().FromNew().AsSingle().WithArguments(_stylizedWaterMaterial);
    }
}