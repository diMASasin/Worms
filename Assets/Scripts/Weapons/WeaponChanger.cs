using System;
using EventProviders;
using UnityEngine;
using WormComponents;

namespace Weapons
{
    public class WeaponChanger: IDisposable
    {
        private readonly IWeaponSelectedEvent _weaponSelectedEvent;
        private readonly IWeaponShotEvent _weaponShotEvent;
        private readonly WeaponView _weaponView;
        private IWormWeapon _currentWorm;

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

        public void ChangeWorm(IWormWeapon worm)
        {
            _currentWorm = worm;
            
            _currentWorm.WeaponRemoved += OnWeaponRemoved;
        }

        private void OnWeaponShot(float shotPower)
        {
            _weaponView.transform.parent = null;
        }

        private void OnWeaponSelected(Weapon weapon)
        {
            Transform weaponViewTransform = _weaponView.transform;
            Transform wormTransform = _currentWorm.WeaponPosition.transform;

            _weaponView.gameObject.SetActive(true);
            weaponViewTransform.parent = wormTransform;
            weaponViewTransform.position = wormTransform.position;
            weaponViewTransform.right = _currentWorm.WeaponPosition.right;
            weapon.Reset();
            
            _currentWorm.ChangeWeapon(weapon);
        }

        private void OnWeaponRemoved()
        {
            _currentWorm.WeaponRemoved -= OnWeaponRemoved;
            
            _weaponView.gameObject.SetActive(false);
        }
    }
}