using System;
using EventProviders;
using UnityEngine;

namespace Weapons
{
    public class WeaponChanger: IDisposable
    {
        private readonly IWeaponSelectedEvent _weaponSelectedEvent;
        private readonly IWeaponShotEvent _weaponShotEvent;
        private readonly WeaponView _weaponView;
        private Worm _currentWorm;

        public WeaponChanger(IWeaponSelectedEvent weaponSelectedEvent, IWeaponShotEvent weaponShotEvent,
            WeaponView weaponView)
        {
            _weaponSelectedEvent = weaponSelectedEvent;
            _weaponShotEvent = weaponShotEvent;
            _weaponView = weaponView;

            _weaponSelectedEvent.WeaponSelected += OnWeaponSelected;
            _weaponShotEvent.WeaponShot += OnWeaponShot;
        }

        public void Dispose()
        {
            _weaponSelectedEvent.WeaponSelected -= OnWeaponSelected;
            _weaponShotEvent.WeaponShot -= OnWeaponShot;
        }

        public void ChangeWorm(Worm worm)
        {
            _currentWorm = worm;
        }

        private void OnWeaponShot(float shotPower)
        {
            _weaponView.transform.parent = null;
        }

        private void OnWeaponSelected(Weapon weapon)
        {
            Transform weaponViewTransform = _weaponView.transform;
            Transform wormTransform = _currentWorm.WeaponPosition.transform;

            weaponViewTransform.parent = wormTransform;
            weaponViewTransform.position = wormTransform.position;
            weaponViewTransform.right = _currentWorm.Armature.right;
            
            _currentWorm.ChangeWeapon(weapon);
        }
    }
}