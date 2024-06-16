using System.Collections.Generic;
using UnityEngine;
using WormComponents;

namespace Weapons
{
    public class MeleeAttacker : MonoBehaviour
    {
        [SerializeField] private Weapon _weapon;
        [SerializeField] private Transform _hitPointStart;
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

        private void Update()
        {
            GetOverlapParameters(out Vector2 origin, out Vector2 pointB);
            Debug.DrawLine(origin, pointB);
        }

        private void GetOverlapParameters(out Vector2 pointA, out Vector2 pointB)
        {
            float length = 1.1f;
            
            pointA = _hitPointStart.transform.position;
            
            pointB = pointA + (Vector2)transform.right * length;
        }
        
        private void OnShot()
        {
            GetOverlapParameters(out Vector2 origin, out Vector2 pointB);
            List<Collider2D> results = new();
            Physics2D.OverlapArea(origin, pointB, _contactFilter, results);
            
            foreach (var result in results)
            {
                if (result.transform.TryGetComponent(out Worm worm) == true)
                {
                    worm.UnfreezePosition();
                    worm.Rigidbody2D.AddForce(transform.right * _weapon.Config.MaxShotPower);
                    worm.TakeDamage(_weapon.Config.Damage);
                }
            }
        }
    }
}