using System;
using UnityEngine;

namespace Weapons
{
    public class ShotPowerIncreaser : MonoBehaviour
    {
        [SerializeField] private Weapon _weapon;

        private IWeaponInput _weaponInput;
        private float _currentShotPower = 0;
        
        public event Action<float> ShotPowerChanged;

        private void Awake()
        {
            _weapon.InputDelegated += OnInputDelegated;
            _weapon.InputRemoved += OnInputRemoved;
        }

        private void OnDestroy()
        {
            _weapon.InputDelegated -= OnInputDelegated;
            _weapon.InputRemoved -= OnInputRemoved;
        }

        private void OnInputDelegated(IWeaponInput weaponInput)
        {
            _weaponInput = weaponInput;
            Reset();
            ShotPowerChanged?.Invoke(_currentShotPower);
            
            _weaponInput.Shoot += OnShot; 
        }

        private void OnInputRemoved()
        {
            if (_weaponInput != null)
            {
                _weaponInput.Shoot -= OnShot;
                _weaponInput = null;
            }

            if (_currentShotPower > 0)
                _weapon.Shoot(_currentShotPower);
        }

        private void Reset()
        {
            _currentShotPower = 0;
        }

        private void Update()
        {
            if (_weaponInput != null && _weaponInput.IsShotPowerIncreasing())
                IncreaseShotPower();
        }

        private void OnShot()
        {
            _weapon.Shoot(_currentShotPower);
            Reset();
        }

        public void IncreaseShotPower()
        {
            if (_currentShotPower >= _weapon.Config.MaxShotPower || _weapon.CanShot == false)
                return;

            _currentShotPower += _weapon.Config.ShotPower * Time.deltaTime;

            if (_currentShotPower >= _weapon.Config.MaxShotPower)
            {
                _currentShotPower = _weapon.Config.MaxShotPower;
                _weapon.Shoot(_currentShotPower);
            }
            ShotPowerChanged?.Invoke(_currentShotPower);
        }
    }
}