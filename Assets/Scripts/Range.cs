using System;
using UnityEngine;

namespace DefaultNamespace
{
    [Serializable] 
    public class Range
    {
        [field: SerializeField] public float StartValue { get; private set; }
        [field: SerializeField] public float EndValue { get; private set; }

        public Range (float startValue, float endValue)
        {
            StartValue = startValue;
            EndValue = endValue;
        }
    }
}