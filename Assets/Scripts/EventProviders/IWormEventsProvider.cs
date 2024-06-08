using System;
using UnityEngine;
using WormComponents;

namespace EventProviders
{
    public interface IWormEvents
    {
        event Action<Worm, Color, string> WormCreated;
        event Action<Worm> WormDamageTook;
        event Action<Worm> WormDied;
    }
}