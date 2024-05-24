using System;
using UnityEngine;
using WormComponents;

namespace EventProviders
{
    public interface IWormEventsProvider
    {
        event Action<IWorm, Color, string> WormCreated;
        event Action<IWorm> WormDamageTook;
        event Action<IWorm> WormDied;
    }
}