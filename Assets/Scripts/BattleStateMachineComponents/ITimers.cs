using Timers;

namespace BattleStateMachineComponents
{
    public interface ITimers
    {
        public Timer BattleTimer { get; set; }
        public Timer TurnTimer { get; set; }
    }
}