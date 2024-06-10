using UnityEngine;

namespace Weapons
{
    public class ScopeMover : MonoBehaviour
    {
        [SerializeField] private Weapon _weapon;
        private IWeaponInput _weaponInput;

        private void OnEnable() => _weapon.InputDelegated += OnInputDelegated;

        private void OnDisable() => _weapon.InputDelegated -= OnInputDelegated;

        private void OnInputDelegated(IWeaponInput weaponInput) => _weaponInput = weaponInput;

        private void Update() => MoveScope();

        public void MoveScope()
        {
            float zRotation = _weaponInput.GetAimDirection() * _weapon.Config.ScopeSensetivity;
            _weapon.transform.Rotate(0, 0, zRotation * Time.deltaTime);
        }
    }
}