using System;
using Services;

namespace Weapons
{
}

namespace Weapons
{
    public interface IWeaponInput : IService
    {
        event Action PointerLineEnabled;
        event Action Shoot;
        float GetAimDirection();
        bool IsShotPowerIncreasing();
    }
}