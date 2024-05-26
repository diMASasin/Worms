using System;
using System.Collections;
using Configs;
using MovementComponents;
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
        int MaxHealth { get; }
        Movement Movement { get; }
        WormConfig Config { get; }
        Transform Transform { get; }
        void Init(WormConfig config);
        void SetRigidbodyKinematic();
        void SetRigidbodyDynamic();
        void SetCurrentWormLayer();
        void SetWormLayer();
        void AddExplosionForce(float explosionForce, Vector3 explosionPosition, float explosionUpwardsModifier);
        void TakeDamage(int damage);
        void Die();
        IEnumerator SetRigidbodyKinematicWhenGrounded();
        void RemoveWeapon();
        event Action<IWorm> Died;
        event Action<IWorm> DamageTook;
    }
}