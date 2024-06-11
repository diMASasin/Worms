using System.Collections.Generic;
using UnityEngine;
using WormComponents;

namespace Weapons
{
    public class MeleeAttacker : MonoBehaviour
    {
        [SerializeField] private Weapon _weapon;
        [SerializeField] private ContactFilter2D _contactFilter;
        [SerializeField] private WeaponAnimatorEventsHandler _animatorEventsHandler;

        private void OnEnable()
        {
            _animatorEventsHandler.WeaponShot += OnShot;
        }

        private void OnDisable()
        {
            _animatorEventsHandler.WeaponShot -= OnShot;
        }

        private void GetCollisionParameters(out Vector2 origin, out Vector2 size)
        {
            origin = transform.position + new Vector3(0.5f, 0);
            size = new Vector2(0.8f, 1);
        }
        
        private void OnShot()
        {
            GetCollisionParameters(out Vector2 origin, out Vector2 size);
            List<RaycastHit2D> results = new();
            Physics2D.BoxCast(origin, size, 0, transform.right, _contactFilter, results);
            
            foreach (var result in results)
            {
                if (result.transform.TryGetComponent(out Worm worm) == true)
                {
                    worm.UnfreezePosition();
                    worm.Rigidbody2D.AddForce(transform.right * _weapon.Config.MaxShotPower);
                    worm.FreezePositionWhenFlyUpAndGrounded();
                }
            }
        }
    }
}