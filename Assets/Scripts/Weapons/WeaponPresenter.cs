using System;
using Projectiles;

namespace Weapons
{
    public class WeaponPresenter : IDisposable
    {
        private Weapon _weapon;

        private readonly WeaponView _weaponView;
        private readonly IWeaponSelectedEventProvider _weaponSelectedEvent;

        public WeaponPresenter(WeaponView weaponView, IWeaponSelectedEventProvider weaponSelectedEvent)
        {
            _weaponView = weaponView;
            _weaponSelectedEvent = weaponSelectedEvent;

            _weaponSelectedEvent.WeaponSelected += OnWeaponChanged;
        }

        public void Dispose()
        {
            TryUnsubscribeWeapon();

            _weaponSelectedEvent.WeaponSelected -= OnWeaponChanged;
        }

        private void OnWeaponChanged(Weapon weapon)
        {
            TryUnsubscribeWeapon();

            _weapon = weapon;

            _weapon.ShotPowerChanged += OnShotPowerChanged;
            _weapon.Shot += OnShot;
            _weapon.PointerLineEnabled += OnPointerLineEnabled;
            _weapon.ScopeMoved += MoveScope;

            _weaponView.SetGunSprite(weapon.Config.Sprite);
            _weaponView.EnableAimSprite();
        }

        private void TryUnsubscribeWeapon()
        {
            if (_weapon == null)
                return;

            _weapon.ShotPowerChanged -= OnShotPowerChanged;
            _weapon.Shot -= OnShot;
            _weapon.PointerLineEnabled -= OnPointerLineEnabled;
            _weapon.ScopeMoved -= MoveScope;
        }

        private void MoveScope(float zRotation)
        {
            _weaponView.MoveScope(zRotation);
        }

        private void OnPointerLineEnabled()
        {
            _weaponView.OnPointerLineEnabled();
        }

        private void OnShot(Projectile projectile, Weapon weapon)
        {
            TryUnsubscribeWeapon();
            _weaponView.OnShot(projectile);
        }

        private void OnShotPowerChanged(float currentShotPower)
        {
            _weaponView.OnShotPowerChanged(currentShotPower / _weapon.Config.MaxShotPower);
        }
    }
}