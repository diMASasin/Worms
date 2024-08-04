using InputService;
using UnityEngine;

namespace Weapons
{
    public interface ILaunchVelocity
    {
        Vector2 GetVelocity();
    }

    class InputLaunchVelocity
    {
        private readonly IWeaponInput _weaponInput;

        public InputLaunchVelocity(IWeaponInput weaponInput)
        {
            _weaponInput = weaponInput;
        }
        
        // public Vector2 GetVelocity()
        // {
        //     
        // }
    }
}