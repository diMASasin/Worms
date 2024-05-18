namespace GameBattleStateMachine.States
{
    public abstract class BattleState
    {
        protected readonly IStateSwitcher StateSwitcher;
        protected readonly BattleStateMachineData Data;
        
        public BattleState(IStateSwitcher stateSwitcher, BattleStateMachineData data)
        {
            StateSwitcher = stateSwitcher;
            Data = data;
        }

        public abstract void Enter();

        public abstract void Exit();

        public abstract void Tick();

        public abstract void HandleInput();
    }
}