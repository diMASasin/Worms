using System;
using BattleStateMachineComponents;
using BattleStateMachineComponents.States;
using Services;

namespace Battle_
{
    public class Battle : IDisposable
    {
        private readonly BattleStateMachine _battleStateMachine; 
    
        public Battle(BattleStateMachineData data, AllServices services)
        {
            _battleStateMachine = new BattleStateMachine(data, services);
        }
        public void Dispose()
        {
            _battleStateMachine.Dispose();
        }

        public void Tick()
        {
            _battleStateMachine.Tick();
        }

        public void LateTick()
        {
            
        }

        public void FixedTick() => _battleStateMachine.FixedTick();
        public void Start() => _battleStateMachine.SwitchState<BootstrapBattleState>();
    }
}