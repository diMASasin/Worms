using System;
using UnityEngine;

namespace Weapons
{
    public class WeaponAnimatorEventsHandler : MonoBehaviour
    {
        [SerializeField] private Weapon _weapon;

        public event Action WeaponShot;
        
        public void OnAnimationEnd() => WeaponShot?.Invoke();

        public void DisableObject() => _weapon.gameObject.SetActive(false);
    }
}