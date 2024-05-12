using System.Collections.Generic;
using Configs;

namespace GameStateMachine
{
    public class BattleStateMachineData
    {
        public readonly TimersConfig TimersConfig;
        public readonly List<Team> AliveTeams = new();

        public int CurrentTeamIndex = -1;
        public Worm CurrentWorm;
        public Team CurrentTeam;
        
        public BattleStateMachineData(TimersConfig timersConfig)
        {
            TimersConfig = timersConfig;
        }
        
        public bool TryGetCurrentTeam(out Team team)
        {
            if (CurrentTeamIndex >= AliveTeams.Count || CurrentTeamIndex < 0)
                team = null;
            else
                team = AliveTeams[CurrentTeamIndex];

            return team != null;
        }
        
        public bool TryGetNextTeam(out Team team)
        {
            CurrentTeamIndex++;

            if (CurrentTeamIndex >= AliveTeams.Count)
                CurrentTeamIndex = 0;

            TryGetCurrentTeam(out team);

            return team != null;
        }
    }
}