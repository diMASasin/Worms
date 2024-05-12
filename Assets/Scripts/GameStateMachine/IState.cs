namespace GameStateMachine
{
    public interface IState
    {
        public void Enter();
        public void Exit();
        public void Tick();
    }
}