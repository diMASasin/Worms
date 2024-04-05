using System;
using System.Collections;
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
    [SerializeField] private WormMovement _wormMovement;
    [SerializeField] private WeaponView _weaponView;

    private Weapon _weapon;
    private PlayerInput _input;

    public int Health { get; private set; }

    public CapsuleCollider2D Collider2D => _collider;
    public Weapon Weapon => _weapon;
    public PlayerInput Input => _input;
    public int MaxHealth => _maxHealth;

    public event UnityAction<int> HealthChanged;
    public event UnityAction<Worm> Died;
    public event UnityAction<Worm> DamageTook;
    public event Action<Weapon> WeaponChanged;

    private void OnEnable()
    {
        _input.InputEnabled += SetRigidbodyDynamic;
        _input.InputDisabled += () => StartCoroutine(SetRigidbodyKinematicWhenGrounded());
    }

    private void OnDisable()
    {
        _input.InputEnabled -= SetRigidbodyDynamic;
        _input.InputDisabled -= () => StartCoroutine(SetRigidbodyKinematicWhenGrounded());
    }

    private void OnDrawGizmos()
    {
        if (_showCanSpawnCheckerBox)
            Gizmos.DrawSphere((Vector2)transform.position + Collider2D.offset, Collider2D.size.x / 2);
    }

    private void Awake()
    {
        Health = _maxHealth;
        _input = new PlayerInput(_wormMovement, this, _weaponView);
    }

    public void Init(Color color, string name)
    {
        _wormInformationView.Init(color, name);
        gameObject.name = name;
    }

    private void Update()
    {
        _input.Tick();
    }

    public void SetRigidbodyKinematic()
    {
        _rigidbody.bodyType = RigidbodyType2D.Kinematic;
    }

    private void SetRigidbodyDynamic()
    {
        _rigidbody.bodyType = RigidbodyType2D.Dynamic;
    }

    private IEnumerator SetRigidbodyKinematicWhenGrounded()
    {
        while (_rigidbody.velocity.magnitude != 0)
            yield return null;

        SetRigidbodyKinematic();
    }

    public void AddExplosionForce(float explosionForce, Vector3 explosionPosition, float explosionUpwardsModifier)
    {
        SetRigidbodyDynamic();
        _wormMovement.AddExplosionForce(explosionForce, explosionPosition, explosionUpwardsModifier);
        StartCoroutine(SetRigidbodyKinematicWhenGrounded());
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
        DamageTook?.Invoke(this);
        HealthChanged?.Invoke(Health);

        if(_input.IsEnabled)
            _input.DisableInput();

        if (Health <= 0)
            Die();
    }

    public void Die()
    {
        Died?.Invoke(this);
        Destroy(gameObject);
    }

    public void ChangeWeapon(Weapon weapon)
    {
        if (_weapon != null)
        {
            if (_weapon.IsShot)
                return;

            RemoveWeapon();
        }

        _weapon = weapon;
        _weaponView.OnGunChanged(_weapon);
        _weapon.SetWorm(this);
        _weapon.Reset();
        WeaponChanged?.Invoke(_weapon);
    }

    public void RemoveWeaponWithDelay()
    {
        StartCoroutine(DelayedRemoveWeapon(_removeWeaponDelay));
    }

    public void RemoveWeapon()
    {
        _weapon = null;
    }

    private IEnumerator DelayedRemoveWeapon(float delay)
    {
        yield return new WaitForSeconds(delay);
        RemoveWeapon();
    }
}
