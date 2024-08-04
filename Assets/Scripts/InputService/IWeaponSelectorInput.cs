using System;

namespace InputService
{
    public interface IWeaponSelectorInput
    {
        public event Action ShouldTogleWeaponSelector;
    }
}