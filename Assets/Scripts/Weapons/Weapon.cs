using System;
using Configs;
using UnityEngine;
using UnityEngine.Events;

namespace Weapons
{
    public class Weapon : MonoBehaviour
    {
        private WeaponConfig _config;

        private float _zRotation;
        private IWeaponInput _weaponInput;

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
            InputDelegated?.Invoke(_weaponInput);
        }

        public void RemoveInput()
        {
            _weaponInput = null;
            InputRemoved?.Invoke();
        }

        public void Reset()
        {
            IsShot = false;
        }

        public void Shoot(float shotPower)
        {
            if (IsShot)
                return;

            IsShot = true;
            Shot?.Invoke(shotPower, this);
        }
    }
}
