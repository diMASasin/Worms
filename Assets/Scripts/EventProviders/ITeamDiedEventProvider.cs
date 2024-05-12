using System;

namespace EventProviders
{
    public interface ITeamDiedEventProvider
    {
        event Action<Team> TeamDied;
    }
}