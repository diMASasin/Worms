using System;
using Configs;
using UnityEngine;
using UnityEngine.Events;

namespace Weapons
{
    public class Weapon : MonoBehaviour
    {
        private WeaponConfig _config;

        private float _currentShotPower = 0;
        private float _zRotation;
        private IWeaponInput _weaponInput;

        public bool IsShot { get; private set; } = false;

        public float CurrentShotPower => _currentShotPower;
        public WeaponConfig Config => _config;
        public GameObject GameObject => gameObject;

        public event Action<float, Weapon> Shot;
        public event Action<float> ShotPowerChanged;
        public event Action IncreasePowerStarted;
        public event Action<float> ScopeMoved;

        public void Init(WeaponConfig config, IWeaponInput weaponInput)
        {
            _weaponInput = weaponInput;
            _config = config;

            _weaponInput.PointerLineEnabled += StartIncresePower;
            _weaponInput.Shoot += Shoot;
        }

        private void OnDestroy()
        {
            _weaponInput.PointerLineEnabled -= StartIncresePower;
            _weaponInput.Shoot -= Shoot;
        }

        private void Update()
        {
            MoveScope();
            
            if(_weaponInput.IsShotPowerIncreasing() == true)
                IncreaseShotPower();
        }

        public void Reset()
        {
            IsShot = false;
            _currentShotPower = 0;
            ShotPowerChanged?.Invoke(_currentShotPower);
        }

        public void MoveScope()
        {
            _zRotation = _weaponInput.GetAimDirection() * Config.ScopeSensetivity;
            //_zRotation = Mathf.Repeat(_zRotation, 720) - 360;
            ScopeMoved?.Invoke(_zRotation);
        }

        public void StartIncresePower()
        {
            if (IsShot)
                return;

            IncreasePowerStarted?.Invoke();
        }

        public void IncreaseShotPower()
        {
            if (_currentShotPower >= _config.MaxShotPower || IsShot)
                return;

            _currentShotPower += _config.ShotPower * Time.deltaTime;

            if (_currentShotPower >= _config.MaxShotPower)
            {
                _currentShotPower = _config.MaxShotPower;
                Shoot();
            }

            ShotPowerChanged?.Invoke(_currentShotPower);
        }
    
        public void Shoot()
        {
            if (IsShot)
                return;

            Shot?.Invoke(_currentShotPower, this);
            IsShot = true;
            _currentShotPower = 0;
        }
    }
}
