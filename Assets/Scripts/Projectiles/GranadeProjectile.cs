using ScriptBoy.Digable2DTerrain;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GranadeProjectile : Projectile
{
    [SerializeField] private float _explodeDelay = 5;
    [SerializeField] private Timer _timer;
    [SerializeField] private FollowingObject _canvas;

    public override void Init(Vector2 value, ExplosionPool explosionPool, Shovel shovel)
    {
        base.Init(value, explosionPool, shovel);
        var torque = Random.Range(5, 7);
        Rigidbody2D.AddTorque(Random.Range(-torque, torque));
        _canvas.transform.parent = null;
        _canvas.gameObject.SetActive(true);

        _timer.StartTimer(_explodeDelay, () =>
        {
            Explode();
            _canvas.gameObject.SetActive(false);
        });
    }
}
