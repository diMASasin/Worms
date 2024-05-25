using System.Collections.Generic;
using Configs;
using EventProviders;
using Pools;
using Weapons;

namespace UI
{
    public interface IWeaponSelectorOpener
    {
        void Toggle();
        void Close();
    }
}