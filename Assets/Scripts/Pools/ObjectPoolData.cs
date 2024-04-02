using System;
using UnityEngine;

namespace Pools
{
    [Serializable]
    public class ObjectPoolData
    {
        [SerializeField] private GameObject _prefab;
        [SerializeField] private int _amount;
    }
}