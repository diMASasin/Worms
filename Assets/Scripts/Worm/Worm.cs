using System;
using System.Collections;
using Configs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

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

    public int Health { get; private set; }
    public Weapon Weapon { get; private set; }
    public PlayerInput Input { get; private set; }

    public CapsuleCollider2D Collider2D => _collider;
    public int MaxHealth => _maxHealth;
    public WeaponView WeaponView => _weaponView;

    public event UnityAction<Worm> Died;
    public event UnityAction<Worm> DamageTook;
    public event Action<Weapon> WeaponChanged;

    private void OnDrawGizmos()
    {
        if (_showCanSpawnCheckerBox)
            Gizmos.DrawSphere((Vector2)transform.position + Collider2D.offset, Collider2D.size.x / 2);
    }

    public void Init(Color color, string wormName)
    {
        gameObject.name = wormName;

        Health = _maxHealth;

        GroundChecker groundChecker = new(transform, Collider2D, _movementConfig.GroundCheckerConfig);
        _wormMovement = new Movement(_rigidbody, _collider, _armature, groundChecker, _movementConfig);

        Input = new PlayerInput(_wormMovement, this, _weaponView);
        _wormInformationView.Init(color, wormName);
        _wormAnimations.Init(groundChecker, _wormMovement);
        
        Input.InputEnabled += SetRigidbodyDynamic;
        Input.InputDisabled += () => StartCoroutine(SetRigidbodyKinematicWhenGrounded());
    }

    private void Update()
    {
        if(Input != null)
            Input.Tick();
    }

    private void OnDestroy()
    {
        Input.InputEnabled -= SetRigidbodyDynamic;
        Input.InputDisabled -= () => StartCoroutine(SetRigidbodyKinematicWhenGrounded());
    }

    public void SetRigidbodyKinematic()
    {
        _rigidbody.bodyType = RigidbodyType2D.Kinematic;
    }

    private void SetRigidbodyDynamic()
    {
        _rigidbody.bodyType = RigidbodyType2D.Dynamic;
    }

    public void OnTurnStarted()
    {
        Input.EnableInput();
        gameObject.layer = (int)math.log2(_currentWormLayerMask.value);
        _arrow.StartMove();
    }

    public void OnTurnEnd()
    {
        RemoveWeaponWithDelay();
        Input.DisableInput();
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
            throw ArgumentOutOfRangeException("damage should be greater then 0");

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
        if (Weapon != null)
        {
            if (Weapon.IsShot)
                return;

            RemoveWeapon();
        }

        Weapon = weapon;
        Weapon.OnAssigned(this, _weaponView.SpawnPoint, _weaponView.transform);
        _weaponView.OnGunChanged(Weapon);
        WeaponChanged?.Invoke(Weapon);
    }

    private IEnumerator SetRigidbodyKinematicWhenGrounded()
    {
        while (_rigidbody.velocity.magnitude != 0)
            yield return null;

        SetRigidbodyKinematic();
    }

    private void RemoveWeaponWithDelay()
    {
        StartCoroutine(DelayedRemoveWeapon(_removeWeaponDelay));
    }

    private void RemoveWeapon()
    {
        Weapon = null;
    }

    private IEnumerator DelayedRemoveWeapon(float delay)
    {
        yield return new WaitForSeconds(delay);
        RemoveWeapon();
    }

    public void StartRoutine(IEnumerator enumerator)
    {
        StartCoroutine(enumerator);
    }
}
