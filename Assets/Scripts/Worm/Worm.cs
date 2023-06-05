using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Worm : MonoBehaviour
{
    [SerializeField] private int _maxHealth = 100;
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private CapsuleCollider2D _collider;
    [SerializeField] private WormInformationView _wormInformationView;
    [SerializeField] private WormInput _input;
    [SerializeField] private Weapon _weapon;
    [SerializeField] private Transform _wormWeaponContainer;
    [SerializeField] private float _removeWeaponDelay = 0.5f;
    [SerializeField] private bool _showCanSpawnCheckerBox = false;
    [SerializeField] private WormMovement _wormMovement;

    public int Health { get; private set; }

    public CapsuleCollider2D Collider2D => _collider;
    public Weapon Weapon => _weapon;
    public WormInput WormInput => _input;
    public int MaxHealth => _maxHealth;

    public event UnityAction<int> HealthChanged;
    public event UnityAction<Worm> Died;
    public event UnityAction<Worm> DamageTook;

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

    private void Start()
    {
        Health = _maxHealth;
    }

    public void Init(Color color, string name)
    {
        _wormInformationView.Init(color, name);
        gameObject.name = name;
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
        //_rigidbody.AddExplosionForce(explosionForce, transform.position, explosionUpwardsModifier);
        _wormMovement.AddExplosionForce(explosionForce, explosionPosition, explosionUpwardsModifier);
        StartCoroutine(SetRigidbodyKinematicWhenGrounded());
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
        DamageTook?.Invoke(this);
        HealthChanged?.Invoke(Health);

        if(_input.enabled)
            _input.DisableInput();

        if (Health <= 0)
            Die();
    }

    public void Die()
    {
        Died?.Invoke(this);
        Destroy(gameObject);
    }

    public void ChangeWeapon(Weapon weapon, Transform weaponContainer)
    {
        if (_weapon)
        {
            if (_weapon.IsShot)
                return;
            TryRemoveWeapon(weaponContainer);
        }

        _weapon = weapon;
        _weapon.transform.parent = _wormWeaponContainer;
        _weapon.transform.localPosition = Vector3.zero;
        _weapon.transform.localEulerAngles = new Vector3(0, 0, 180);
        _weapon.SetWorm(this);
        _weapon.Reset();
        _weapon.gameObject.SetActive(true);
        _input.ChangeWeapon(weapon);
    }

    public void RemoveWeaponWithDelay(Transform weaponContainer)
    {
        StartCoroutine(DelayedRemoveWeapon(weaponContainer, _removeWeaponDelay));
    }

    public void TryRemoveWeapon(Transform weaponContainer)
    {
        if (_weapon == null)
            return;

        _weapon.transform.parent = weaponContainer;
        _weapon.gameObject.SetActive(false);
        _weapon = null;
    }

    private IEnumerator DelayedRemoveWeapon(Transform weaponContainer, float delay)
    {
        yield return new WaitForSeconds(delay);
        TryRemoveWeapon(weaponContainer);
    }
}
