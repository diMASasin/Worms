using System.Collections.Generic;
using System.Linq;
using BattleStateMachineComponents.States;

namespace BattleStateMachineComponents
{
    public class BattleStateMachine : IStateSwitcher
    {
        private readonly BattleStateMachineData _data;
        private readonly List<BattleState> _states;
        private BattleState _currentState;

        public BattleStateMachine(BattleStateMachineData data)
        {
            _data = data;
            _states = new List<BattleState>()
            {
                new StartBattleState(this, data),
                new BetweenTurnsState(this, data),
                new TurnState(this, data),
                new RetreatState(this, data),
                new ProjectilesWaiting(this, data)
            };

            _currentState = _states[0];
            _currentState.Enter();
        }

        public void SwitchState<T>() where T : BattleState
        {
            BattleState state = _states.FirstOrDefault(state => state is T);

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
            _data.WindMediator.FixedTick();
        }

        public void Dispose()
        {
            _data.Dispose();
        }
    }
}