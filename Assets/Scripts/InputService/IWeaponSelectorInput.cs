using System;
using Services;

namespace UI
{
    public interface IWeaponSelectorInput : IService
    {
        public event Action ShouldTogleWeaponSelector;
    }
}