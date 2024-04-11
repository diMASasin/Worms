using UnityEngine;

public class WindEffect : MonoBehaviour
{
    [SerializeField] private ParticleSystem _particleSystem;
    private Wind _wind;

    public void Init(Wind wind)
    {
        _wind = wind;

        _wind.VelocityChanged += OnVelocityChanged;
    }

    private void OnDestroy()
    {
        _wind.VelocityChanged -= OnVelocityChanged;
    }

    private void OnVelocityChanged(float velocity)
    {
        var velocityOverLifetime = _particleSystem.velocityOverLifetime;
        velocityOverLifetime.x = -velocity;
    }
}
