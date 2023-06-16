using ScriptBoy.Digable2DTerrain;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Projectile : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private CircleCollider2D _collider2D;
    [SerializeField] private GameObject _spriteObject;
    [SerializeField] private int _damage;
    [SerializeField] private float _explosionForce = 10;
    [SerializeField] private float _explosionUpwardsModifier = 4;
    [SerializeField] private float _explosionRadius = 1;

    private Shovel _shovel;
    private bool _dead;
    private ExplosionPool _explosionPool;

    public Rigidbody2D Rigidbody2D => _rigidbody;
    public GameObject SpriteRenderer => _spriteObject;

    public event UnityAction<Projectile> Exploded;

    public virtual void Init(ExplosionPool explosionPool, Shovel shovel) 
    {
        _explosionPool = explosionPool;
        _shovel = shovel;
    }

    public void SetVelocity(Vector2 velocity)
    {
        _rigidbody.velocity = velocity;
    }

    public void Reset()
    {
        _dead = false;
    }

    public abstract void OnShot();

    public void Explode()
    {
        if (_dead) 
            return;

        _dead = true;
        _shovel.radius = _explosionRadius;
        _shovel.transform.position = transform.position;
        _shovel.Dig();
        var explosion = _explosionPool.Get();
        explosion.transform.position = transform.position;
        explosion.Explode(_damage, _collider2D.radius, _explosionForce, 
            _explosionUpwardsModifier, _explosionRadius, () => _explosionPool.Remove(explosion));
        Exploded?.Invoke(this);
    }
}
