using UI;

namespace BattleStateMachineComponents.States
{
    public class BattleEndState : IBattleState
    {
        private readonly EndScreen _endScreen;
        private readonly BattleStateMachineData _data;

        public BattleEndState(BattleStateMachine battleStateMachine, EndScreen endScreen)
        {
            _endScreen = endScreen;
        }

        public void Enter()
        {
            _endScreen.Show();
        }

        public void Exit()
        {
        }

        public void Tick()
        {
        }

        public void HandleInput()
        {
        }
    }
}