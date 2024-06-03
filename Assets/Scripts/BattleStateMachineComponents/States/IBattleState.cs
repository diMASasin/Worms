namespace BattleStateMachineComponents.States
{
    public interface IBattleState
    {
        void Enter();
        void Exit();
        void Tick();
        void FixedTick();
        void LateTick();
        void HandleInput();
    }
}