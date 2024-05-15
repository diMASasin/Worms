using System;

public interface IWeaponSelectedEvent
{
    public event Action<Weapon> WeaponSelected;
}