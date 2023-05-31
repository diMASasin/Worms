using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Projectile : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private Explosion _explosion;
    [SerializeField] private CircleCollider2D _collider2D;
    [SerializeField] private GameObject _spriteObject;

    private Cutter _cut;
    private bool _dead;
    private ExplosionPool _explosionPool;

    public Rigidbody2D Rigidbody2D => _rigidbody;
    public GameObject SpriteRenderer => _spriteObject;

    public event UnityAction<Projectile> Exploded;

    public virtual void Init(Vector2 value, ExplosionPool explosionPool) 
    {
        _rigidbody.velocity = value;
        _explosionPool = explosionPool;
    }

    private void Start()
    {
        _cut = FindObjectOfType<Cutter>();
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        DelayedExplode();
    }

    public void Reset()
    {
        _dead = false;
    }

    public void DelayedExplode(float delay = 0)
    {
        if (_dead) return;
        Invoke(nameof(Explode), delay);

        _dead = true;
    }

    public void Explode()
    {
        _cut.transform.position = transform.position;
        Invoke(nameof(DoCut), 0.01f);
    }

    private void DoCut() 
    {
        _cut.DoCut();
        var explosion = _explosionPool.Get();
        explosion.transform.position = transform.position;
        explosion.Explode(_collider2D.radius, () => _explosionPool.Remove(explosion));
        Exploded?.Invoke(this);
    }
}
