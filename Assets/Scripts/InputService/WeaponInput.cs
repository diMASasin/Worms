using System;
using Zenject;
using static UnityEngine.InputSystem.InputAction;

namespace InputService
{
    class WeaponInput : IWeaponInput, IDisposable, IInitializable
    {
        private MainInput.WeaponActions _weaponInput;

        public event Action PowerIncreasingStarted;
        public event Action Shoot;

        public WeaponInput(MainInput.WeaponActions weaponActions)
        {
            _weaponInput = weaponActions;
            _weaponInput.Enable();
        }

        public void Initialize()
        {
            _weaponInput.EnablePointerLine.performed += EnablePointerLine;
            _weaponInput.Shoot.performed += OnShoot;
        }

        public void Dispose()
        {
            

            _weaponInput.EnablePointerLine.performed -= EnablePointerLine;
            _weaponInput.Shoot.performed -= OnShoot;
        }

        public float GetAimDirection() => -_weaponInput.RaiseScope.ReadValue<float>();

        private void EnablePointerLine(CallbackContext callbackContext) => PowerIncreasingStarted?.Invoke();

        private void OnShoot(CallbackContext obj) => Shoot?.Invoke();

        public bool IsShotPowerIncreasing() => _weaponInput.IncreaseShotPower.ReadValue<float>() == 1;
    }
}