using WormComponents;

namespace BattleStateMachineComponents.StatesData
{
    public interface IAliveTeams
    {
        public CycledList<Team> AliveTeams { get; }
    }
}