using System;
using WormComponents;

namespace EventProviders
{
    public interface ITeamDiedEventProvider
    {
        event Action<Team> TeamDied;
    }
}