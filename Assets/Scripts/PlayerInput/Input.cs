using System;

namespace PlayerInput
{
    public class Input : IDisposable
    {
        private Worm _worm;
        private Weapon _weapon;

        private readonly MovementInput _movementInput;
        private readonly WeaponInput _weaponInput;
        
        public Input(MainInput input)
        {
            input.Enable();
            _movementInput = new MovementInput(input.Movement);
            _weaponInput = new WeaponInput(input.Weapon);
        }
        
        public void Dispose()
        {
            UnsubscribeWormIfExists();

            _movementInput.Disable();
            _weaponInput.Disable();
        }

        public void Tick()
        {
            _movementInput.Tick();
            _weaponInput.Tick();
        }

        public void Enable(Worm worm)
        {
            ChangeWorm(worm);
            _movementInput.Enable(worm.Movement);
            _worm.SetRigidbodyDynamic();
        }

        public void Disable()
        {
            if(_worm == null)
                return;
        
            _movementInput.Disable();
            _weaponInput.Disable();
            CoroutinePerformer.StartCoroutine(_worm.SetRigidbodyKinematicWhenGrounded());
            _worm.Movement.Reset();
        }

        private void ChangeWorm(Worm newWorm)
        {
            UnsubscribeWormIfExists();

            _worm = newWorm;

            if (_worm != null)
            {
                _worm.WeaponChanged += OnWeaponChanged;
                _worm.WeaponRemoved += OnWeaponRemoved;
                _worm.DamageTook += OnDamageTook;
            }
        }

        private void UnsubscribeWormIfExists()
        {
            if (_worm == null)
                return;
            
            _worm.WeaponChanged -= OnWeaponChanged;
            _worm.WeaponRemoved -= OnWeaponRemoved;
            _worm.DamageTook -= OnDamageTook;
        }

        private void OnWeaponChanged(Weapon weapon)
        {
            _weaponInput.Enable(weapon);
        }

        private void OnWeaponRemoved()
        {
            _weaponInput.Disable();
        }

        private void OnDamageTook(Worm worm)
        {
            _weaponInput.Disable();
            _movementInput.Disable();
        }
    }
}
