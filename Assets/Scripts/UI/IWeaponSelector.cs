using System.Collections.Generic;
using EventProviders;
using Weapons;

namespace UI
{
    public interface IWeaponSelector : IWeaponSelectedEvent
    {
        void Init(List<Weapon> weaponList);
        void Toggle();
        void Close();
    }
}