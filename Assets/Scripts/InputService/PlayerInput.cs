using System;
using CameraFollow;
using EventProviders;
using UI;
using Weapons;
using WormComponents;

namespace InputService
{
    public class PlayerInput : IDisposable
    {
        private readonly IWormEventsProvider _wormEvents;
        private Worm _worm;
        private Weapon _weapon;

        public readonly MovementInput MovementInput;
        public readonly WeaponInput WeaponInput;
        public readonly CameraInput CameraInput;
        public readonly UIInput UIInput;
        
        public PlayerInput(MainInput input, IControllableCamera camera, WeaponSelector weaponSelector)
        {
            input.Enable();
            MovementInput = new MovementInput(input.Movement);
            WeaponInput = new WeaponInput(input.Weapon);
            CameraInput = new CameraInput(camera);
            UIInput = new UIInput(weaponSelector);
        }
        
        public void Dispose()
        {
            UnsubscribeWormIfExists();

            MovementInput.Disable();
            WeaponInput.Disable();
        }

        public void OnWormChanged(Worm worm)
        {
            ChangeWorm(worm);
            MovementInput.Enable(worm.Movement);
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
            WeaponInput.Enable(weapon);
        }

        private void OnWeaponRemoved()
        {
            WeaponInput.Disable();
        }

        private void OnDamageTook(Worm worm)
        {
            WeaponInput.Disable();
            MovementInput.Disable();
        }
    }
}
