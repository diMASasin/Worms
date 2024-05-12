using System;

public interface IWeaponSelectedEventProvider
{
    public event Action<Weapon> WeaponSelected;
}