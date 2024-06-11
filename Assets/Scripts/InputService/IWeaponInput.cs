using System;
using Services;

namespace Weapons
{
}

namespace Weapons
{
    public interface IWeaponInput : IService
    {
        event Action PowerIncreasingStarted;
        event Action Shoot;
        float GetAimDirection();
        bool IsShotPowerIncreasing();
    }
}