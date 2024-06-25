using System;
using UnityEngine.InputSystem;

namespace UI
{
    class WeaponSelectorInput : IWeaponSelectorInput, IDisposable
    {
        private readonly MainInput.UIActions _uiActions;
        
        public event Action ShouldTogleWeaponSelector;

        public WeaponSelectorInput(MainInput.UIActions uiActions)
        {
            _uiActions = uiActions;
            _uiActions.Enable();
        }
        
        public void Subscribe()
        {
            _uiActions.OpenWeaponSelector.performed += OpenWeaponSelector;
        }

        public void Dispose()
        {
            _uiActions.OpenWeaponSelector.performed -= OpenWeaponSelector;
        }

        private void OpenWeaponSelector(InputAction.CallbackContext obj) => ShouldTogleWeaponSelector?.Invoke();

    }
}