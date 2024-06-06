using System;
using EventProviders;
using Pools;
using UnityEngine;
using WormComponents;

namespace Weapons
{
    public class WeaponChanger: IDisposable
    {
        private readonly IWeaponSelectedEvent _weaponSelectedEvent;
        private readonly IWeaponShotEvent _weaponShotEvent;
        private readonly Transform _weaponsParent;
        private IWormWeapon _currentWorm;

        public WeaponChanger(IWeaponSelectedEvent weaponSelectedEvent, IWeaponShotEvent weaponShotEvent,
             Transform weaponsParent)
        {
            _weaponSelectedEvent = weaponSelectedEvent;
            _weaponShotEvent = weaponShotEvent;
            _weaponsParent = weaponsParent;

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

        private void OnWeaponShot(float shotPower, Weapon weapon)
        {
            weapon.transform.parent = _weaponsParent;
        }

        private void OnWeaponSelected(Weapon weapon)
        {
            Transform weaponTransform = weapon.transform;
            Transform wormTransform = _currentWorm.WeaponPosition.transform;

            ((Component)weapon).gameObject.SetActive(true);
            weaponTransform.parent = wormTransform;
            weaponTransform.position = wormTransform.position;
            weaponTransform.right = _currentWorm.WeaponPosition.right;
            weapon.Reset();
            
            _currentWorm.ChangeWeapon(weapon);
        }

        private void OnWeaponRemoved(IWeapon weapon)
        {
            _currentWorm.WeaponRemoved -= OnWeaponRemoved;
            
            weapon.gameObject.SetActive(false);
        }
    }
}