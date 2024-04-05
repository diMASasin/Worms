using System;
using UnityEngine;

namespace Pools
{
    [Serializable]
    public class ObjectPoolData<T>
    {
        [field: SerializeField] public T Prefab { get; set; }
        [field: SerializeField] public int Amount { get; set; } = 1;
    }
}