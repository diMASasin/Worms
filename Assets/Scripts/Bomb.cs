using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    Cutter _cut;
    bool _dead;
    [SerializeField] Rigidbody2D _rigidbody;
    [SerializeField] GameObject _explosionPrefab;

    public void SetVelocity(Vector2 value) {
        _rigidbody.velocity = value;
        _rigidbody.AddTorque(Random.Range(-8f,8f));
    }

    private void Start()
    {
        _cut = FindObjectOfType<Cutter>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_dead) return;
        _cut.transform.position = transform.position;
        Invoke(nameof(DoCut), 0.001f);
        
        _dead = true;
    }

    void DoCut() {
        _cut.DoCut();
        Destroy(gameObject);
        Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
    }

}
