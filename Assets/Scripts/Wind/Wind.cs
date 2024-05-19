using System;
using Random = UnityEngine.Random;

namespace Wind
{
    public class Wind
    {
        private WindData _data;

        public float Velocity { get; set; }
        public float MaxVelocity => _data.MaxVelocity;
        public float Step => _data.Step;

        public event Action<float> VelocityChanged;

        public Wind(WindData data)
        {
            _data = data;
        }

        public void ChangeVelocity()
        {
            Velocity = Random.Range(-MaxVelocity, MaxVelocity);
            Velocity = Velocity - Velocity % Step;
            VelocityChanged?.Invoke(Velocity);
        }
    }
}
