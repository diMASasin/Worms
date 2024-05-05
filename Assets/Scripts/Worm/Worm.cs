using System;
using System.Collections;
using Configs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using Weapons;

public class Worm : MonoBehaviour
{
    [SerializeField] private int _maxHealth = 100;
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private CapsuleCollider2D _collider;
    [SerializeField] private WormInformationView _wormInformationView;
    [SerializeField] private float _removeWeaponDelay = 0.5f;
    [SerializeField] private bool _showCanSpawnCheckerBox = false;
    [SerializeField] private MovementConfig _movementConfig;
    [SerializeField] private Movement _wormMovement;
    [SerializeField] private WeaponView _weaponView;
    [SerializeField] private LayerMask _wormLayerMask;
    [SerializeField] private LayerMask _currentWormLayerMask;
    [SerializeField] private Arrow _arrow;
    [SerializeField] private Transform _armature;
    [SerializeField] private WormAnimations _wormAnimations;

    private GroundChecker _groundChecker;

    public int Health { get; private set; }
    public Weapon Weapon { get; private set; }

    public CapsuleCollider2D Collider2D => _collider;
    public int MaxHealth => _maxHealth;
    public WeaponView WeaponView => _weaponView;
    public Movement Movement => _wormMovement;

    public event UnityAction<Worm> Died;
    public event UnityAction<Worm> DamageTook;
    public event Action<Weapon> WeaponChanged;
    public event Action<Color, string> Initialized;

    private void OnDrawGizmos()
    {
        if (_showCanSpawnCheckerBox)
            Gizmos.DrawSphere((Vector2)transform.position + Collider2D.offset, Collider2D.size.x / 2);
    }

    public void Init(Color color, string wormName)
    {
        gameObject.name = wormName;

        Health = _maxHealth;

        _groundChecker = new GroundChecker(transform, Collider2D, _movementConfig.GroundCheckerConfig);
        _wormMovement = new Movement(_rigidbody, _collider, _armature, _groundChecker, _movementConfig);

        _wormInformationView.Init(color, wormName);
        _wormAnimations.Init(_groundChecker, _wormMovement);

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
        gameObject.layer = (int)math.log2(_currentWormLayerMask.value);
        _arrow.StartMove();
    }

    public void OnTurnEnd()
    {
        RemoveWeaponWithDelay();
        gameObject.layer = (int)math.log2(_wormLayerMask.value);
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

        if (Weapon != null)
        {
            Weapon.OnAssigned(this, _weaponView.SpawnPoint, _weaponView.transform);
            _weaponView.OnWeaponChanged(Weapon);
            WeaponPresenter presenter = new WeaponPresenter(Weapon, _weaponView);
        }

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
        yield return new WaitForSeconds(_removeWeaponDelay);
        RemoveWeapon();
    }
}
