using System;
using System.Collections;
using Configs;
using MovementComponents;
using UltimateCC;
using UnityEngine;
using UnityEngine.Events;
using Weapons;

namespace WormComponents
{
    public interface IWorm : IWormWeapon
    {
        Transform Armature { get; }
        Transform WeaponPosition { get; }
        int Health { get; }
        IWeapon Weapon { get; }
        CapsuleCollider2D Collider2D { get; }
        PlayerInputManager Input { get; }
        int MaxHealth { get; }
        WormConfig Config { get; }
        Transform Transform { get; }
        void Init(WormConfig config);
        void FreezePosition();
        void UnfreezePosition();
        void FreezePositionWhenGrounded();
        void SetCurrentWormLayer();
        void SetWormLayer();
        void AddExplosionForce(float explosionForce, Vector3 explosionPosition, float upwardsModifier,
            float explosionRadius);
        void TakeDamage(int damage);
        void Die();
        void RemoveWeapon();
        event Action<IWorm> Died;
        event Action<IWorm> DamageTook;
    }
}