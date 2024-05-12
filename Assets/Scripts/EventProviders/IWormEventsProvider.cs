using System;

namespace EventProviders
{
    public interface IWormEventsProvider
    {
        event Action<Worm> WormDamageTook;
        event Action<Worm> WormDied;
    }
}