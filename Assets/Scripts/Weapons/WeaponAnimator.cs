using UnityEngine;

namespace Weapons
{
    public class WeaponAnimator : MonoBehaviour
    {
        [SerializeField] private Weapon _weapon;
        [SerializeField] private Animator _animator;

        private static readonly int Attack = Animator.StringToHash("Attack");
        
        private void OnEnable()
        {
            _weapon.Shot += OnShot;
        }

        private void OnDisable()
        {
            _weapon.Shot -= OnShot;
        }

        private void OnShot(float velocity, Weapon weapon) => _animator.SetTrigger(Attack);
    }
}