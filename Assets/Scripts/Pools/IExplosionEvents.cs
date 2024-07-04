using System;
using Explosion_;

namespace Pools
{
    public interface IExplosionEvents
    {
        event Action<Explosion> Exploded;
        event Action<Explosion> AnimationStopped;
    }
}