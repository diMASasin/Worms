using System;
using BattleStateMachineComponents;
using BattleStateMachineComponents.States;
using BattleStateMachineComponents.StatesData;
using UI;
using Wind_;

namespace Battle_
{
    public class Battle : IDisposable
    {
        private readonly BattleStateMachine _battleStateMachine; 
    
        public Battle(BattleStateMachineData data, TurnStateData turnStateData, StartStateData startStateData,
            BetweenTurnsStateData windMediator, EndScreen endScreen)
        {
            _battleStateMachine = new BattleStateMachine(data, turnStateData, windMediator, startStateData, endScreen);
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

        public void Start() => _battleStateMachine.SwitchState<StartBattleState>();

        public void OnDrawGizmos() => _battleStateMachine.OnDrawGizmos();
    }
}