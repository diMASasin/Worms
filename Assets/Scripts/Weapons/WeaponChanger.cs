using System;
using BattleStateMachineComponents.StatesData;
using EventProviders;
using InputService;
using UI_;
using UnityEngine;
using WormComponents;

namespace Weapons
{
    public class WeaponChanger : IDisposable
    {
        private readonly IWeaponSelectedEvent _weaponSelectedEvent;
        private readonly IWeaponShotEvent _weaponShotEvent;
        private readonly Transform _weaponsParent;
        private readonly IWormEvents _wormEvents;
        private readonly ICurrentWorm _currentWormProvider;
        private readonly IWeaponInput _weaponInput;
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
            _wormEvents.WormDied -= OnWormDied;
        }

        private void OnPowerIncreasingStarted()
        {
            if (CurrentWeapon != null && CurrentWeapon.CanShot == true)
                _weaponSelector.DisallowOpen();
        }

        private void OnSelectorOpened() => CurrentWeapon?.DisallowShoot();

        private void OnSelectorClosed() => CurrentWeapon?.AllowShoot();

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
            CurrentWeapon.gameObject.SetActive(false);
            WeaponRemoved?.Invoke(weapon);
        }

        private void OnWormDied(Worm worm)
        {
            RemoveWeapon(CurrentWeapon);
        }
    }
}