using System;
using System.Collections;
using Configs;
using MovementComponents;
using UltimateCC;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using Weapons;

namespace WormComponents
{
    public class Worm : MonoBehaviour, IWorm
    {
        [SerializeField] private Rigidbody2D _rigidbody;
        [SerializeField] private CapsuleCollider2D _collider;
        [field: SerializeField] public PlayerInputManager Input { get; private set; }
        [field: SerializeField] public Transform Armature { get; private set; }
        [field: SerializeField] public Transform WeaponPosition { get; private set; }

        public WormConfig Config { get; private set; }
        public int Health { get; private set; }
        public IWeapon Weapon { get; private set; }

        public Transform Transform => transform;
        public CapsuleCollider2D Collider2D => _collider;
        public int MaxHealth => Config.MaxHealth;

        public static int WormsNumber;

        public event Action<IWorm> Died;
        public event Action<IWorm> DamageTook;
        public event Action<IWeapon> WeaponChanged;
        public event Action<IWeapon> WeaponRemoved;
        
        private void OnDrawGizmos()
        {
            var colliderSize = Collider2D.size;
            var size = new Vector2(colliderSize.x * 2, colliderSize.y);
            
            if (Config != null && Config.ShowCanSpawnCheckerBox)
                Gizmos.DrawCube(transform.position + new Vector3(0, 0.5f), size);
                // Gizmos.DrawSphere(transform.position + (Vector3)Collider2D.offset, Collider2D.size.x / 2);
        }

        public void Init(WormConfig config)
        {
            Config = config;
            WormsNumber++;
            gameObject.name = config.Name + " " + WormsNumber;
            Health = Config.MaxHealth;
            Input.Disable();
        }

        private void OnDestroy()
        {
        }

        public void FreezePosition()
        {
            // _rigidbody.bodyType = RigidbodyType2D.Kinematic;
            _rigidbody.constraints = RigidbodyConstraints2D.FreezePosition;
        }

        public void UnfreezePosition()
        {
            // _rigidbody.bodyType = RigidbodyType2D.Dynamic;
            _rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        public void SetCurrentWormLayer() => gameObject.layer = (int)math.log2(Config.CurrentWormLayerMask.value);

        public void SetWormLayer() => gameObject.layer = (int)math.log2(Config.WormLayerMask.value);


        public void AddExplosionForce(float explosionForce, Vector3 explosionPosition, float upwardsModifier,
            float explosionRadius)
        {
            UnfreezePosition();
            _rigidbody.AddExplosionForce(explosionForce, explosionPosition, explosionRadius, upwardsModifier);
            FreezePositionWhenGrounded();
        }

        public void TakeDamage(int damage)
        {
            if (damage < 0)
                throw new ArgumentOutOfRangeException("damage should be greater then 0. damage = " + damage);

            Health -= damage;
            Input.Disable();
            DamageTook?.Invoke(this);

            if (Health <= 0)
                Die();
        }

        public void Die()
        {
            RemoveWeapon();
            Died?.Invoke(this);
            Destroy(gameObject);
        }

        public void FreezePositionWhenGrounded() => StartCoroutine(FreezePositionWhenGroundedCoroutine());
        private IEnumerator FreezePositionWhenGroundedCoroutine()
        {
            do
            {
                yield return null;
                
                if (_rigidbody == null) yield break;
                
            } while (_rigidbody.velocity.magnitude != 0);

            FreezePosition();
        }

        public void RemoveWeapon()
        {
            WeaponRemoved?.Invoke(Weapon);
            ChangeWeapon(null);
        }

        public void ChangeWeapon(IWeapon weapon)
        {
            Weapon = weapon;

            WeaponChanged?.Invoke(Weapon);
        }
    }
}
