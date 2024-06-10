using System.Collections.Generic;
using System.Linq;
using BattleStateMachineComponents.States;
using Services;
using UnityEngine;

namespace BattleStateMachineComponents
{
    public class BattleStateMachine : IStateSwitcher
    {
        private readonly BattleStateMachineData _data;
        private readonly BootstrapBattleState _bootstrapBattleState;
        private readonly List<IBattleState> _states;
        private IBattleState _currentState;

        public BattleStateMachine(BattleStateMachineData data, AllServices services)
        {
            _data = data;
            _bootstrapBattleState = new BootstrapBattleState(this, data, services); 
            
            _states = new List<IBattleState>()
            {
                _bootstrapBattleState,
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

        public void Tick()
        {
            _currentState.Tick();
        }

        public void FixedTick() => _currentState.FixedTick();

        public void Dispose() => _bootstrapBattleState.Dispose();
    }
}