using System;

namespace UI_
{
    public interface IWeaponSelectorEvents
    {
        event Action SelectorOpened;
        event Action SelectorClosed;
    }
}