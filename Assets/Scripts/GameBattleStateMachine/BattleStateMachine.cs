using System.Collections.Generic;
using System.Linq;
using Configs;
using GameBattleStateMachine.States;
using GameStateMachine;

namespace GameBattleStateMachine
{
    public class BattleStateMachine : IStateSwitcher
    {
        private readonly List<IBattleState> _states;
        private IBattleState _currentState;

        public BattleStateMachine(TimersConfig timersConfig)
        {
            BattleStateMachineData data = new BattleStateMachineData(timersConfig);

            _states = new List<IBattleState>()
            {
                new NotBattle(this, data),
                new BetweenTurnsState(this, data),
                new TurnState(this, data),
                new RetreatState(this, data)
            };

            _currentState = _states[0];
            _currentState.Enter();
        }

        public void SwitchState<T>() where T : IBattleState
        {
            IBattleState state = _states.FirstOrDefault(state => state is T);

            _currentState?.Exit();
            _currentState = state;
            _currentState.Enter();
        }

        // public void HandleInput() => _currentState.HandleInput();

        public void Tick() => _currentState.Tick();
    }
}