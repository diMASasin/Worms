using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindEffect : MonoBehaviour
{
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private Wind _wind;

    private void OnValidate()
    {
        _wind = FindObjectOfType<Wind>();
    }
    private void OnEnable()
    {
        _wind.VelocityChanged += OnVelocityChanged;
    }

    private void OnDisable()
    {
        _wind.VelocityChanged -= OnVelocityChanged;
    }

    private void OnVelocityChanged(float velocity)
    {
        var velocityOverLifetime = _particleSystem.velocityOverLifetime;
        velocityOverLifetime.x = -velocity;
    }
}
