using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class Worm : MonoBehaviour
{
    [SerializeField] private int _maxHealth = 100;
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private CapsuleCollider2D _collider;
    [SerializeField] private float _speed = 2f;
    [SerializeField] private float _jumpSpeed = 5f;
    [SerializeField] private Animator _animator;
    [SerializeField] private Transform _wormSprite;
    [SerializeField] private WormInformationView _wormInformationView;
    [SerializeField] private WormInput _input;
    [SerializeField] private Throwing _throwing;
    [SerializeField] private bool _showCanSpawnCheckerBox = false;

    private int _health;

    public CapsuleCollider2D Collider2D => _collider;
    public Throwing Throwing => _throwing;
    public WormInput WormInput => _input;

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
        _health = _maxHealth;
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

    public void AddExplosionForce(float explosionForce, Transform transform, float explosionUpwardsModifier)
    {
        SetRigidbodyDynamic();
        _rigidbody.AddExplosionForce(explosionForce, transform.position, explosionUpwardsModifier);
        StartCoroutine(SetRigidbodyKinematicWhenGrounded());
    }

    public void TakeDamage(int damage)
    {
        _health -= damage;
        DamageTook?.Invoke(this);
        HealthChanged?.Invoke(_health);

        if (_health <= 0)
            Die();
    }

    public void Die()
    {
        Died?.Invoke(this);
        Destroy(gameObject);
    }
}
