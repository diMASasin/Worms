using System;

namespace _UI
{
    public interface IWeaponSelectorEvents
    {
        event Action SelectorOpened;
        event Action SelectorClosed;
    }
}