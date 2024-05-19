using UnityEngine.InputSystem;
using Weapons;

namespace InputService
{
    public class WeaponInput
    {
        private readonly MainInput.WeaponActions _weaponInput;
        private Weapon _weapon;

        public WeaponInput(MainInput.WeaponActions weaponInput)
        {
            _weaponInput = weaponInput;
        }
        
        public void Enable(Weapon weapon)
        {
            _weapon = weapon;
            
            _weaponInput.Enable();
            
            _weaponInput.EnablePointerLine.performed += OnEnablePointerLine;
            _weaponInput.Shoot.performed += OnShoot;
        }

        public void Disable()
        {
            if(_weapon == null)
                return;
            
            _weaponInput.Disable();
            
            _weaponInput.EnablePointerLine.performed -= OnEnablePointerLine;
            _weaponInput.Shoot.performed -= OnShoot;
        }

        public void Tick()
        {
            if(_weapon == null)
                return;
            
            OnAimDirectionChanged();
            OnIncreaseShotPower();
        }

        private void OnShoot(InputAction.CallbackContext obj) => _weapon?.Shoot();

        private void OnEnablePointerLine(InputAction.CallbackContext obj) => _weapon?.StartIncresePower();
        
        private void OnAimDirectionChanged()
        {
            var direction = _weaponInput.RaiseScope.ReadValue<float>();
            _weapon.MoveScope(direction);
        }

        private void OnIncreaseShotPower()
        {
            var isIncreasing = _weaponInput.IncreaseShotPower.ReadValue<float>();
            if(isIncreasing == 1)
                _weapon.IncreaseShotPower();
        }
    }
}