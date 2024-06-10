using System;
using BattleStateMachineComponents.StatesData;
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
        private readonly ICurrentWorm _currentWormProvider;
        private readonly IWeaponInput _weaponInput;
        private Transform _weaponTransform;

        public Weapon CurrentWeapon { get; private set; }

        public event Action<Weapon> WeaponRemoved;
        public event Action<Weapon> WeaponChanged;

        public WeaponChanger(IWeaponSelectedEvent weaponSelectedEvent, IWeaponShotEvent weaponShotEvent,
            Transform weaponsParent, IWormEvents wormEvents, ICurrentWorm currentWormProvider, IWeaponInput weaponInput)
        {
            _weaponSelectedEvent = weaponSelectedEvent;
            _weaponShotEvent = weaponShotEvent;
            _weaponsParent = weaponsParent;
            _wormEvents = wormEvents;
            _currentWormProvider = currentWormProvider;
            _weaponInput = weaponInput;

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

        private void OnWeaponShot(float shotPower, Weapon weapon) => TryRemoveWeapon(weapon);

        private void OnWeaponSelected(Weapon weapon)
        {
            TryRemoveWeapon(CurrentWeapon);

            Transform wormTransform = _currentWormProvider.CurrentWorm.WeaponPosition.transform;
            _weaponTransform = weapon.transform;
            CurrentWeapon = weapon;

            weapon.GameObject.SetActive(true);
            _weaponTransform.parent = wormTransform;
            _weaponTransform.position = wormTransform.position;
            _weaponTransform.right = _currentWormProvider.CurrentWorm.WeaponPosition.right;
            weapon.DelegateInput(_weaponInput);
            weapon.Reset();
            
            WeaponChanged?.Invoke(weapon);
        }

        public void TryRemoveWeapon(Weapon weapon)
        {
            if (weapon == null)
                return;
            
            _weaponTransform.parent = _weaponsParent;
            weapon.RemoveInput();
            weapon.GameObject.SetActive(false);
            WeaponRemoved?.Invoke(weapon);
        }

        private void OnWormDied(Worm worm)
        {
            TryRemoveWeapon(CurrentWeapon);
        }
    }
}