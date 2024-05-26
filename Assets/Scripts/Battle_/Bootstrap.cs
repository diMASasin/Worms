using BattleStateMachineComponents;
using BattleStateMachineComponents.StatesData;
using Infrastructure;
using UI;
using UnityEngine;
using PlayerInput = InputService.PlayerInput;

namespace Battle_
{
    public class Bootstrap : MonoBehaviour
    {
        [SerializeField] private CoroutinePerformer _coroutinePerformer;
        [SerializeField] private BattleStateMachineData _data;

        [field: SerializeField] public BetweenTurnsStateData BetweenTurnsStateData { get; private set; }
        [field: SerializeField] public StartStateData StartStateData { get; private set; }
        [field: SerializeField] public TurnStateData TurnStateData { get; private set; }
        [field: SerializeField] public EndScreen EndScreen { get; private set; }
        
        private Battle _battle;
        private MainInput _mainInput;
        private PlayerInput _input;
    
        private void Start()
        {
            _coroutinePerformer.Init();
            _mainInput = new MainInput();
        
            _data.Init(_mainInput, StartStateData.GameConfig.TimersConfig);
            _battle = new Battle(_data, TurnStateData, StartStateData, BetweenTurnsStateData, EndScreen);
        
            _battle.Start();
        }
    
        private void Update()
        {
            _battle?.Tick();
        }

        private void FixedUpdate()
        {
            _battle?.FixedTick();
        }

        private void OnDrawGizmos()
        {
            _battle?.OnDrawGizmos();
        }

        private void OnDestroy()
        {
            if (_battle != null) _battle.Dispose();
        }
    }
}
