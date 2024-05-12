using System;
using System.Collections;
using Configs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

public class Worm : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private CapsuleCollider2D _collider;
    [SerializeField] private Arrow _arrow;
    [SerializeField] private Transform _armature;
    [SerializeField] private WormAnimations _wormAnimations;

    private WormConfig _wormConfig;
    private Movement _wormMovement;
    private GroundChecker _groundChecker;

    public int Health { get; private set; }
    public Weapon Weapon { get; private set; }

    public CapsuleCollider2D Collider2D => _collider;
    public int MaxHealth => _wormConfig.MaxHealth;
    public Movement Movement => _wormMovement;

    public event UnityAction<Worm> Died;
    public event UnityAction<Worm> DamageTook;
    public event Action<Weapon> WeaponChanged;
    public event Action<Color, string> Initialized;

    private void OnDrawGizmos()
    {
        if (_wormConfig.ShowCanSpawnCheckerBox)
            Gizmos.DrawSphere((Vector2)transform.position + Collider2D.offset, Collider2D.size.x / 2);
    }

    public void Init(Color color, string wormName, WormConfig config)
    {
        _wormConfig = config;
        gameObject.name = wormName;
        Health = _wormConfig.MaxHealth;

        _groundChecker = new GroundChecker(transform, Collider2D, _wormConfig.MovementConfig.GroundCheckerConfig);
        _wormMovement = new Movement(_rigidbody, _collider, _armature, _groundChecker, _wormConfig.MovementConfig);

        _wormAnimations.Init(_groundChecker, _wormMovement);
        
        SetRigidbodyKinematic();

        Initialized?.Invoke(color, wormName);
    }

    private void FixedUpdate()
    {
        _wormMovement.FixedTick();
        _groundChecker.FixedTick();
    }

    private void OnDestroy()
    {
    }

    public void SetRigidbodyKinematic()
    {
        _rigidbody.bodyType = RigidbodyType2D.Kinematic;
    }

    public void SetRigidbodyDynamic()
    {
        _rigidbody.bodyType = RigidbodyType2D.Dynamic;
    }

    public void OnTurnStarted()
    {
        gameObject.layer = (int)math.log2(_wormConfig.CurrentWormLayerMask.value);
        _arrow.StartMove();
    }

    public void OnTurnEnd()
    {
        RemoveWeaponWithDelay();
        gameObject.layer = (int)math.log2(_wormConfig.WormLayerMask.value);
    }

    public void AddExplosionForce(float explosionForce, Vector3 explosionPosition, float explosionUpwardsModifier)
    {
        SetRigidbodyDynamic();
        _wormMovement.AddExplosionForce(explosionForce, explosionPosition, explosionUpwardsModifier);
        StartCoroutine(SetRigidbodyKinematicWhenGrounded());
    }

    public void TakeDamage(int damage)
    {
        if (damage < 0)
            throw new ArgumentOutOfRangeException("damage should be greater then 0. damage = " + damage);

        Health -= damage;
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

    public void ChangeWeapon(Weapon weapon)
    {
        Weapon = weapon;

        Weapon?.Reset();

        WeaponChanged?.Invoke(Weapon);
    }

    public IEnumerator SetRigidbodyKinematicWhenGrounded()
    {
        while (_rigidbody.velocity.magnitude != 0)
            yield return null;

        SetRigidbodyKinematic();
    }

    private void RemoveWeaponWithDelay()
    {
        StartCoroutine(DelayedRemoveWeapon());
    }

    private void RemoveWeapon()
    {
        ChangeWeapon(null);
    }

    private IEnumerator DelayedRemoveWeapon()
    {
        yield return new WaitForSeconds(_wormConfig.RemoveWeaponDelay);
        RemoveWeapon();
    }
}
