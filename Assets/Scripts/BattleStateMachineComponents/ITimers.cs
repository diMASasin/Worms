using Timers;

namespace BattleStateMachineComponents
{
    public interface ITimers
    {
        public ITimer BattleTimer { get; set; }
        public ITimer TurnTimer { get; set; }
    }
}