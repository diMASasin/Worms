using System;
using BattleStateMachineComponents.StatesData;
using EventProviders;
using Pools;
using UI;
using UnityEngine;
using WormComponents;
using Zenject;

namespace Weapons
{
    public class WeaponChanger : IDisposable
    {
        private IWeaponSelectedEvent _weaponSelectedEvent;
        private IWeaponShotEvent _weaponShotEvent;
        private readonly Transform _weaponsParent;
        private IWormEvents _wormEvents;
        private ICurrentWorm _currentWormProvider;
        private IWeaponInput _weaponInput;
        private readonly WeaponSelector _weaponSelector;

        private Transform _weaponTransform;

        public Weapon CurrentWeapon { get; private set; }

        public event Action<Weapon> WeaponRemoved;
        public event Action<Weapon> WeaponChanged;

        public WeaponChanger(WeaponSelector weaponSelector, Transform weaponsParent, IWeaponInput weaponInput, 
            IWormEvents wormEvents, IWeaponShotEvent weaponShotEvent,IWeaponSelectedEvent weaponSelectedEvent, 
            ICurrentWorm currentWormProvider)
        {
            _weaponsParent = weaponsParent;
            _weaponSelector = weaponSelector;
            _weaponInput = weaponInput;
            _wormEvents = wormEvents;
            _weaponShotEvent = weaponShotEvent;
            _weaponSelectedEvent = weaponSelectedEvent;
            _currentWormProvider = currentWormProvider;
            
            _weaponInput.PowerIncreasingStarted += OnPowerIncreasingStarted;
            _wormEvents.WormDied += OnWormDied;
            _weaponShotEvent.WeaponShot += OnWeaponShot;
            _weaponSelectedEvent.WeaponSelected += OnWeaponSelected;
            _weaponSelector.SelectorOpened += OnSelectorOpened;
            _weaponSelector.SelectorClosed += OnSelectorClosed;
        }

        public void Dispose()
        {
            _weaponInput.PowerIncreasingStarted -= OnPowerIncreasingStarted;
            _weaponSelector.SelectorOpened -= OnSelectorOpened;
            _weaponSelector.SelectorClosed -= OnSelectorClosed;
            _weaponSelectedEvent.WeaponSelected -= OnWeaponSelected;
            _weaponShotEvent.WeaponShot -= OnWeaponShot;
            _wormEvents.WormDied -= OnWormDied;
        }

        private void OnPowerIncreasingStarted()
        {
            if (CurrentWeapon != null && CurrentWeapon.CanShot == true)
                _weaponSelector.DisallowOpen();
        }

        private void OnSelectorOpened() => CurrentWeapon?.DisallowShoot();

        private void OnSelectorClosed() => CurrentWeapon?.AllowShoot();

        private void OnWeaponShot(float shotPower, Weapon weapon) => RemoveWeapon(weapon);

        private void OnWeaponSelected(Weapon weapon)
        {
            Transform wormTransform = _currentWormProvider.CurrentWorm.WeaponPosition.transform;
            _weaponTransform = weapon.transform;
            RemoveWeapon(CurrentWeapon);
            
            CurrentWeapon?.gameObject.SetActive(false);
            
            CurrentWeapon = weapon;

            weapon.GameObject.SetActive(true);
            _weaponTransform.parent = wormTransform;
            _weaponTransform.position = wormTransform.position;
            _weaponTransform.right = _currentWormProvider.CurrentWorm.WeaponPosition.right;
            weapon.DelegateInput(_weaponInput);
            weapon.AllowShoot();

            WeaponChanged?.Invoke(weapon);
        }

        public void RemoveWeapon(Weapon weapon)
        {
            if (_weaponTransform == null || weapon == null)
                return;

            weapon.RemoveInput();
            _weaponTransform.parent = _weaponsParent;
            WeaponRemoved?.Invoke(weapon);
        }

        private void OnWormDied(Worm worm)
        {
            RemoveWeapon(CurrentWeapon);
        }
    }
}