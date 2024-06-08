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
        private readonly IWormEvents _wormEvents;
        private Worm _currentWorm;
        private Transform _weaponTransform;

        public Weapon CurrentWeapon { get; private set; }

        public event Action<Weapon> WeaponRemoved;
        public event Action<Weapon> WeaponChanged;

        public WeaponChanger(IWeaponSelectedEvent weaponSelectedEvent, IWeaponShotEvent weaponShotEvent,
             Transform weaponsParent, IWormEvents wormEvents)
        {
            _weaponSelectedEvent = weaponSelectedEvent;
            _weaponShotEvent = weaponShotEvent;
            _weaponsParent = weaponsParent;
            _wormEvents = wormEvents;

            _weaponSelectedEvent.WeaponSelected += OnWeaponSelected;
            _weaponShotEvent.WeaponShot += OnWeaponShot;
            _wormEvents.WormDied += OnWormDied;
        }

        public void Dispose()
        {
            _weaponSelectedEvent.WeaponSelected -= OnWeaponSelected;
            _weaponShotEvent.WeaponShot -= OnWeaponShot;
            _wormEvents.WormDied -= OnWormDied;
        }

        private void OnWeaponShot(float shotPower, Weapon weapon)
        {
            weapon.transform.parent = _weaponsParent;
        }

        private void OnWeaponSelected(Weapon weapon)
        {
            Transform wormTransform = _currentWorm.WeaponPosition.transform;
            _weaponTransform = weapon.transform;

            RemoveWeapon(CurrentWeapon);
            CurrentWeapon = weapon;
            
            weapon.GameObject.SetActive(true);
            _weaponTransform.parent = wormTransform;
            _weaponTransform.position = wormTransform.position;
            _weaponTransform.right = _currentWorm.WeaponPosition.right;
            weapon.Reset();
            
            WeaponChanged?.Invoke(weapon);
        }

        private void RemoveWeapon(Weapon weapon)
        {
            _weaponTransform.parent = _weaponsParent;
            weapon.GameObject.SetActive(false);
            WeaponRemoved?.Invoke(weapon);
        }

        private void OnWormDied(Worm worm)
        {
            RemoveWeapon(CurrentWeapon);
        }
    }
}