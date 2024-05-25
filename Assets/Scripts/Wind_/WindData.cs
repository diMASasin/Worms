using System;
using UnityEngine;

namespace Wind_
{
    [Serializable]
    public class WindData
    {
        [field: SerializeField] public float MaxVelocity { get; private set; } = 2;
        [field: SerializeField] public float Step { get; private set; } = 0.2f;

        [field: SerializeField] public ParticleSystem Particles { get; private set; }
    }
}