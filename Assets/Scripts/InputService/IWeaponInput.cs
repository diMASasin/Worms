using System;

namespace InputService
{
    public interface IWeaponInput
    {
        event Action PowerIncreasingStarted;
        event Action Shoot;
        float GetAimDirection();
        bool IsShotPowerIncreasing();
    }
}