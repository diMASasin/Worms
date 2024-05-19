using System;
using Configs;
using UnityEngine;
using UnityEngine.Events;

namespace Weapons
{
    public class Weapon
    {
        private readonly WeaponConfig _config;

        private float _currentShotPower = 0;
        private float _zRotation;

        public bool IsShot { get; private set; } = false;

        public float CurrentShotPower => _currentShotPower;
        public WeaponConfig Config => _config;

        public event UnityAction<float> Shot;
        public event UnityAction<float> ShotPowerChanged;
        public event UnityAction IncreasePowerStarted;
        public event Action<float> ScopeMoved;

        public Weapon(WeaponConfig config)
        {
            _config = config;
        }

        public void Reset()
        {
            IsShot = false;
            _currentShotPower = 0;
        }

        public void MoveScope(float direction)
        {
            _zRotation = -direction * Config.ScopeSensetivity;
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

            Shot?.Invoke(_currentShotPower);
            IsShot = true;
            _currentShotPower = 0;
        }
    }
}
