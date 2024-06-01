using System.Collections.Generic;
using System.Linq;
using BattleStateMachineComponents.States;

namespace BattleStateMachineComponents
{
    public class BattleStateMachine : IStateSwitcher
    {
        private readonly BattleStateMachineData _data;
        private readonly StartBattleState _startBattleState;
        private readonly List<IBattleState> _states;
        private IBattleState _currentState;

        public BattleStateMachine(BattleStateMachineData data)
        {
            _data = data;
            _startBattleState = new StartBattleState(this, data); 
            
            _states = new List<IBattleState>()
            {
                _startBattleState,
                new BetweenTurnsState(this, data.GlobalBattleData, data.BetweenTurnsData),
                new TurnState(this, data.GlobalBattleData, data.TurnStateData),
                new RetreatState(this, data.GlobalBattleData),
                new ProjectilesWaiting(this),
                new BattleEndState(this, data.EndScreen)
            };
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
            _data.GlobalBattleData.PlayerInput.CameraInput.Tick();
        }

        public void FixedTick()
        {
            _startBattleState.FixedTick();
        }

        public void LateTick()
        {
            _startBattleState.LateTick();
        }

        public void Dispose()
        {
            _startBattleState.Dispose();
        }
    }
}