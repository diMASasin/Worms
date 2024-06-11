using UnityEngine;

namespace Weapons
{
    public class StaticShotPower : MonoBehaviour
    {
        [SerializeField] private Weapon _weapon;

        private IWeaponInput _weaponInput;
        
        private void OnEnable()
        {
            _weapon.InputDelegated += OnInputDelegated;
            _weapon.InputRemoved += OnInputRemoved;
        }

        private void OnDisable()
        {
            _weapon.InputDelegated -= OnInputDelegated;
            _weapon.InputRemoved -= OnInputRemoved;
        }

        private void OnInputDelegated(IWeaponInput weaponInput)
        {
            _weaponInput = weaponInput;
            _weaponInput.PowerIncreasingStarted += OnPowerIncreasingStarted;
        }

        private void OnInputRemoved()
        {
            _weaponInput.PowerIncreasingStarted -= OnPowerIncreasingStarted;
        }

        private void OnPowerIncreasingStarted()
        {
            _weapon.Shoot(_weapon.Config.MaxShotPower); 
        }
    }
}