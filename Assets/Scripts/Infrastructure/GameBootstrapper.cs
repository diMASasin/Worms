using GameStateMachineComponents;
using GameStateMachineComponents.States;
using UnityEngine;
using Zenject;

namespace Infrastructure
{
    public class GameBootstrapper : MonoBehaviour
    {
        private GameStateMachine _stateMachine;
        private static GameBootstrapper _instance;

        [Inject]    
        public void Construct(GameStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }
        
        private void Start()
        {
            _stateMachine.Init();
            _stateMachine.SwitchState<BootstrapState>();
            
            DontDestroyOnLoad(gameObject);
        }
    }
}