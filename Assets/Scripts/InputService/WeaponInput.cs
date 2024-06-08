using System;
using UnityEngine.InputSystem;

namespace Weapons
{
    class WeaponInput : IWeaponInput
    {
        private MainInput.WeaponActions _weaponInput;

        public event Action PointerLineEnabled;
        public event Action Shoot;

        public WeaponInput(MainInput.WeaponActions weaponActions)
        {
            _weaponInput = weaponActions;
            _weaponInput.Enable();
        }
        
        public void Subscribe()
        {
            _weaponInput.EnablePointerLine.performed += EnablePointerLine;
            _weaponInput.Shoot.performed += OnShoot;
        }

        public void Unsubscribe()
        {
            _weaponInput.EnablePointerLine.performed -= EnablePointerLine;
            _weaponInput.Shoot.performed -= OnShoot;
        }

        public float GetAimDirection() => -_weaponInput.RaiseScope.ReadValue<float>();
        private void EnablePointerLine(InputAction.CallbackContext callbackContext) => PointerLineEnabled?.Invoke();
        private void OnShoot(InputAction.CallbackContext obj) => Shoot?.Invoke();
        public bool IsShotPowerIncreasing() => _weaponInput.IncreaseShotPower.ReadValue<float>() == 1;
    }
}