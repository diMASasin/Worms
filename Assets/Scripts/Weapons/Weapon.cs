using System;
using Configs;
using InputService;
using UnityEngine;

namespace Weapons
{
    public class Weapon : MonoBehaviour
    {
        private WeaponConfig _config;

        private float _zRotation;
        private IWeaponInput _weaponInput;

        public bool CanShot { get; private set; } = false;
        public bool IsShot { get; private set; } = false;

        public WeaponConfig Config => _config;
        public GameObject GameObject => gameObject;

        public event Action<float, Weapon> Shot;
        public event Action<IWeaponInput> InputDelegated;
        public event Action InputRemoved;

        public void Init(WeaponConfig config)
        {
            _config = config;
        }

        public void DelegateInput(IWeaponInput weaponInput)
        {
            _weaponInput = weaponInput;
            IsShot = false;
            InputDelegated?.Invoke(_weaponInput);
        }

        public void RemoveInput()
        {
            _weaponInput = null;
            InputRemoved?.Invoke();
        }

        public void AllowShoot() => CanShot = true;

        public void DisallowShoot() => CanShot = false;

        public void Shoot(float shotPower)
        {
            if (CanShot == false || IsShot == true)
                return;

            IsShot = true;
            CanShot = false;
            Shot?.Invoke(shotPower, this);
        }
    }
}
