using System;
using Configs;
using UnityEngine;

public class Projectile
{
    private bool _dead;
    
    public Vector2 Velocity { get; private set; }

    public event Action<Projectile> Exploded;
    public event Action Reseted;
    public event Action<Vector2> Launched;

    public void Reset()
    {
        Reseted?.Invoke();
        Velocity = Vector2.zero;
        _dead = false;
    }

    public void InfluenceOnVelocity(Vector2 additionalVelocity)
    {
        Velocity += additionalVelocity;
    }
    
    public void Explode()
    {
        if (_dead) 
            return;

        _dead = true;

        Exploded?.Invoke(this);
    }

    public void Launch(Vector2 velocity)
    {
        Velocity = velocity;
        Launched?.Invoke(Velocity);
    }
}