using System;
using Random = UnityEngine.Random;

namespace Wind_
{
    public class Wind
    {
        public readonly float MaxVelocity;
        public readonly float Step;

        public float Velocity { get; private set; }
        public float NormalizedVelocity => Velocity / MaxVelocity;
        
        public event Action<float> VelocityChanged;

        public Wind(float maxVelocity, float step)
        {
            MaxVelocity = maxVelocity;
            Step = step;
        }

        public void ChangeVelocity()
        {
            Velocity = Random.Range(-MaxVelocity, MaxVelocity);
            Velocity -= Velocity % Step;
            VelocityChanged?.Invoke(Velocity);
        }
    }
}
