using UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace InputService
{
    public class UIInput
    {
        private readonly MainInput.UIActions _uiActions;
        private readonly WeaponSelector _weaponSelector;

        public UIInput(MainInput.UIActions uiActions, WeaponSelector weaponSelector)
        {
            _uiActions = uiActions;
            _weaponSelector = weaponSelector;
            
        }

        public void Enable()
        {
            _uiActions.OpenWeaponSelector.performed += OpenWeaponSelector;
        }

        public void Disable()
        {
            _uiActions.OpenWeaponSelector.performed -= OpenWeaponSelector;
        }
        
        private void OpenWeaponSelector(InputAction.CallbackContext obj)
        {
            _weaponSelector.Toggle();
        }
    }
}