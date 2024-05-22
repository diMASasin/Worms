using System;
using BattleStateMachineComponents;
using BattleStateMachineComponents.States;

namespace Battle_
{
    public class Battle : IDisposable
    {
        private readonly BattleStateMachine _battleStateMachine; 
    
        public Battle(BattleStateMachineData data)
        {
            _battleStateMachine = new BattleStateMachine(data);
        }
        public void Dispose()
        {
            _battleStateMachine.Dispose();
        }

        public void Tick()
        {
            _battleStateMachine.Tick();
            _battleStateMachine.HandleInput();
        }

        public void FixedTick() => _battleStateMachine.FixedTick();

        public void Start() => _battleStateMachine.SwitchState<BetweenTurnsState>();

        public void OnDrawGizmos() => _battleStateMachine.OnDrawGizmos();
    }
}