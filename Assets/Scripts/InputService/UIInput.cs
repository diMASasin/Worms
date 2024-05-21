using UI;
using UnityEngine;

namespace InputService
{
    public class UIInput
    {
        private const int RightButton = 1;
        
        private readonly WeaponSelector _weaponSelector;

        public UIInput(WeaponSelector weaponSelector)
        {
            _weaponSelector = weaponSelector;
        }
        
        public void Tick()
        {
            if (Input.GetMouseButtonDown(RightButton)) 
                _weaponSelector.Toggle();
        }
    }
}