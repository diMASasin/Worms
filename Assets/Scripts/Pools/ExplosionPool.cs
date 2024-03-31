using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ExplosionPool : MonoBehaviour
{
    [SerializeField] private Explosion _explosionTemplate;
    [SerializeField] private int _amount;

    private List<Explosion> _explosions = new();
    private List<Explosion> _used = new();

    private void Start()
    {
        for (int i = 0; i < _amount; i++)
            CreateExplosion();
    }

    public Explosion Get()
    {
        if (_used.Count == _explosions.Count)
            CreateExplosion();

        var explosion = _explosions.First(explosion => !_used.Contains(explosion));

        _used.Add(explosion);
        explosion.gameObject.SetActive(true);
        return explosion;
    }

    public void Remove(Explosion explosion)
    {
        explosion.transform.parent = transform;
        _used.Remove(explosion);
    }

    private Explosion CreateExplosion()
    {
        var explosion = Instantiate(_explosionTemplate, transform);
        _explosions.Add(explosion);
        return explosion;
    }
}
