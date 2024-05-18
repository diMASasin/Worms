using System;
using UnityEngine;
using WormComponents;

namespace EventProviders
{
    public interface IWormEventsProvider
    {
        event Action<Worm, Color, string> WormCreated;
        event Action<Worm> WormDamageTook;
        event Action<Worm> WormDied;
    }
}