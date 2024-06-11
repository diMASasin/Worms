using System;
using UnityEngine;
using WormComponents;

namespace EventProviders
{
    public interface IWormEvents
    {
        event Action<Worm, Color, string> WormCreated;
        event Action<Worm> DamageTook;
        event Action<Worm> WormDied;
        event Action<Worm> InputDelegated;
        event Action<Worm> InputRemoved;
    }
}