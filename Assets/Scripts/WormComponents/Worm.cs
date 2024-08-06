using System;
using _2D_Ultimate_Side_Scroller_Character_Controller.Scripts;
using _2D_Ultimate_Side_Scroller_Character_Controller.Scripts.Input_System;
using _2D_Ultimate_Side_Scroller_Character_Controller.Scripts.Input_System.InputManager;
using Configs;
using Extensions;
using R3;
using Unity.Mathematics;
using UnityEngine;

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

        private readonly ReactiveProperty<int> _health = new();
        private bool _isDied;
        
        private WormConfig Config { get; set; }

        public ReadOnlyReactiveProperty<int> Health => _health;
        public Transform Transform => transform;
        public CapsuleCollider2D Collider2D => _collider;
        public Rigidbody2D Rigidbody2D => _rigidbody;
        public int MaxHealth => Config.MaxHealth;

        public static int WormsNumber;

        public event Action<Worm> Died;
        public event Action<Worm> DamageTook;
        public event Action<Worm> InputDelegated;
        public event Action<Worm> InputRemoved;
        
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
            _health.Value = Config.MaxHealth;
        }

        private void OnDestroy() => _health.Dispose();

        public void DelegateInput(IMovementInput movementInput)
        {
            SetCurrentWormLayer();
            UnfreezePosition();
            InputHandler.Enable(movementInput);
            InputDelegated?.Invoke(this);
        }
        
        public void RemoveInput()
        {
            InputHandler.Disable();
            InputRemoved?.Invoke(this);
        }

        public void UnfreezePosition() => _rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;

        public void SetCurrentWormLayer() => gameObject.layer = (int)math.log2(Config.CurrentWormLayerMask.value);

        public void SetWormLayer() => gameObject.layer = (int)math.log2(Config.WormLayerMask.value);
        
        public void AddExplosionForce(Vector3 direction, float explosionForce, float forceMultiplier, float upwardsModifier)
        {
            UnfreezePosition();
            _rigidbody.AddExplosionForce(direction, explosionForce, forceMultiplier, upwardsModifier);
        }

        public void TakeDamage(int damage)
        {
            if (damage < 0)
                throw new ArgumentOutOfRangeException("damage should be greater then 0. damage = " + damage);

            _health.Value -= damage;
            _playerMain.PlayerData.Physics.Attacked = true;
            InputHandler.Disable();
            DamageTook?.Invoke(this);

            if (_health.Value <= 0)
                Die();
        }

        public void Die()
        {
            _isDied = true;
            _playerMain.PlayerData.Physics.died = true;
            Died?.Invoke(this);
            Destroy(gameObject);
        }
    }
}
