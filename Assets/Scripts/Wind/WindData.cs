using System;
using UnityEngine;

namespace DefaultNamespace.Wind
{
    [Serializable]
    public class WindData
    {
        [field: SerializeField] public float MaxVelocity { get; private set; } = 2;
        [field: SerializeField] public float Step { get; private set; } = 0.1f;
    }
}