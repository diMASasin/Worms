using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Bomb : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private GameObject _explosionPrefab;
    [SerializeField] private Explosion _explosion;
    [SerializeField] private CircleCollider2D _collider2D;

    private Cutter _cut;
    private bool _dead;

    public event UnityAction<Bomb> Exploded;

    public void SetVelocity(Vector2 value) 
    {
        _rigidbody.velocity = value;
        _rigidbody.AddTorque(Random.Range(-8f,8f));
    }

    private void Start()
    {
        _cut = FindObjectOfType<Cutter>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Explode();
    }

    public void Explode()
    {
        if (_dead) return;
        _cut.transform.position = transform.position;
        Invoke(nameof(DoCut), 0.001f);

        _dead = true;
    }

    private void DoCut() 
    {
        _cut.DoCut();
        _explosion.Explode(_collider2D.radius);
        Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        Exploded?.Invoke(this);
        Destroy(gameObject);
    }
}
