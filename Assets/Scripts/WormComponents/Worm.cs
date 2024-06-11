using System;
using System.Collections;
using Configs;
using UltimateCC;
using Unity.Mathematics;
using UnityEngine;
using Weapons;

namespace WormComponents
{
    public class Worm : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D _rigidbody;
        [SerializeField] private CapsuleCollider2D _collider;
        [SerializeField] private PlayerMain _playerMain;
        [field: SerializeField] public InputHandler InputHandler { get; private set; }
        [field: SerializeField] public Transform Armature { get; private set; }
        [field: SerializeField] public Transform WeaponPosition { get; private set; }

        private bool _isDied;
        public WormConfig Config { get; private set; }
        public int Health { get; private set; }
        public Weapon Weapon { get; private set; }

        public Transform Transform => transform;
        public CapsuleCollider2D Collider2D => _collider;
        public Rigidbody2D Rigidbody2D => _rigidbody;
        public int MaxHealth => Config.MaxHealth;

        public static int WormsNumber;

        public event Action<Worm> Died;
        public event Action<Worm> DamageTook;
        
        private void OnDrawGizmosSelected()
        {
            var colliderSize = Collider2D.size;
            var size = new Vector2(colliderSize.x * 2, colliderSize.y);
            
            if (Config != null && Config.ShowCanSpawnCheckerBox)
                Gizmos.DrawCube(transform.position + new Vector3(0, 0.5f), size);
        }

        public void Init(WormConfig config)
        {
            Config = config;
            WormsNumber++;
            gameObject.name = config.Name + " " + WormsNumber;
            Health = Config.MaxHealth;
        }

        public void DelegateInput(IMovementInput movementInput)
        {
            SetCurrentWormLayer();
            UnfreezePosition();
            InputHandler.Enable(movementInput);
        }
        
        public void RemoveInput()
        {
            SetWormLayer();
            if(_isDied == false) FreezePositionWhenGrounded();
            InputHandler.Disable();
        }
        
        public void FreezePosition() => _rigidbody.constraints = RigidbodyConstraints2D.FreezePosition;

        public void UnfreezePosition() => _rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;

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
            InputHandler.Disable();
            DamageTook?.Invoke(this);

            if (Health <= 0)
                Die();
        }

        public void Die()
        {
            _isDied = true;
            Died?.Invoke(this);
            Destroy(gameObject, 1);
        }

        public void FreezePositionWhenGrounded() => StartCoroutine(FreezePositionWhenGroundedCoroutine());
        private IEnumerator FreezePositionWhenGroundedCoroutine()
        {
           
            while (_playerMain.PlayerData.Physics.IsGrounded == false)
                yield return null;
            
            FreezePosition();
        }
        
        public void FreezePositionWhenFlyUpAndGrounded() => StartCoroutine(FreezePositionWhenFlyUpAndGroundedCoroutine());
        private IEnumerator FreezePositionWhenFlyUpAndGroundedCoroutine()
        {
            while (_playerMain.PlayerData.Physics.IsGrounded == true)
                yield return null;
            
            while (_playerMain.PlayerData.Physics.IsGrounded == false)
                yield return null;
            
            FreezePosition();
        }
    }
}
