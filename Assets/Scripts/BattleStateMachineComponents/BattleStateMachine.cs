using System.Collections.Generic;
using System.Linq;
using BattleStateMachineComponents.States;
using BattleStateMachineComponents.StatesData;
using UI;

namespace BattleStateMachineComponents
{
    public class BattleStateMachine : IStateSwitcher
    {
        private readonly BattleStateMachineData _data;
        private readonly StartBattleState _startBattleState;
        private readonly List<IBattleState> _states;
        private IBattleState _currentState;

        public BattleStateMachine(BattleStateMachineData data, TurnStateData turnStateData,
            BetweenTurnsStateData betweenTurnsData, StartStateData startStateData, EndScreen endScreen)
        {
            _data = data;
            _startBattleState = new StartBattleState(this, data, startStateData, turnStateData, betweenTurnsData); 
            
            _states = new List<IBattleState>()
            {
                _startBattleState,
                new BetweenTurnsState(this, data, betweenTurnsData),
                new TurnState(this, data, turnStateData),
                new RetreatState(this, data),
                new ProjectilesWaiting(this),
                new BattleEndState(this, endScreen)
            };

            _currentState = _startBattleState;
            _currentState.Enter();
        }

        public void SwitchState<T>() where T : IBattleState
        {
            IBattleState state = _states.FirstOrDefault(state => state is T);

            _currentState?.Exit();
            _currentState = state;
            _currentState.Enter();
        }

        public void HandleInput()
        {
            _currentState.HandleInput();
        }

        public void Tick()
        {
            _currentState.Tick();
            _data.PlayerInput.CameraInput.Tick();
        }

        public void FixedTick()
        {
            _startBattleState.FixedTick();
        }

        public void OnDrawGizmos()
        {
            _startBattleState.OnDrawGizmos();
        }

        public void Dispose()
        {
            _startBattleState.Dispose();
        }
    }
}