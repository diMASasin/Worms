using System;

namespace UI
{
    public interface IWeaponSelectorEvents
    {
        event Action SelectorOpened;
        event Action SelectorClosed;
    }
}