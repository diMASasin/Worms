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
        
        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
            
            _stateMachine?.Init();
            _stateMachine?.SwitchState<BootstrapState>();
            
            DontDestroyOnLoad(gameObject);
        }
    }
}