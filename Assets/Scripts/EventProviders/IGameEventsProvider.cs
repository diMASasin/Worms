using System;
using WormComponents;

namespace EventProviders
{
    public interface IGameEventsProvider
    {
        public event Action<Worm, Team> TurnStarted;
        public event Action TurnEnd;
        public event Action GameStarted;
        public event Action GameEnd;
    }
}