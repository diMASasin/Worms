using Infrastructure;
using InputService;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameStateMachineComponents.States
{
    public class BootstrapState : GameState
    {
        private SceneLoader SceneLoader => Data.SceneLoader;

        public BootstrapState(GameStateMachineData data, IGameStateSwitcher stateSwitcher) : 
            base(data, stateSwitcher) { }

        public override void Enter()
        {
            SceneLoader.Load(SceneLoader.MainMenu, OnLoaded);
        }

        private void OnLoaded()
        {
            if(SceneManager.GetActiveScene().name == SceneLoader.MainMenu)
                StateSwitcher.SwitchState<LevelLoadState>();
        }

        public override void Exit()
        {
        }

        public override void Tick()
        {
        }
    }
}