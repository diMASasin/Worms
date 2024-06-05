using Battle_;
using Infrastructure;
using Services;
using UnityEngine;

namespace GameStateMachineComponents.States
{
    public class BootstrapState : GameState
    {
        private readonly ISceneLoader _sceneLoader;

        public BootstrapState(GameStateMachineData data, IGameStateSwitcher stateSwitcher, AllServices services) : 
            base(data, stateSwitcher)
        {
            RegisterServices(services);
            
            _sceneLoader = services.Single<ISceneLoader>();
        }

        public override void Enter()
        {            
            Data.LoadingScreen.Init(_sceneLoader);
            
            _sceneLoader.Load(_sceneLoader.SceneNames.MainMenu, OnLoaded);
        }

        private void OnLoaded()
        {
            StateSwitcher.SwitchState<MainMenuState>();
            // if(SceneManager.GetActiveScene().name == SceneLoader.MainMenu)
            //     StateSwitcher.SwitchState<LevelLoadState>();
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
        }
    }
}