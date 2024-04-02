using Pools;
using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "PoolsConfig", menuName = "PoolsConfig", order = 0)]
    public class PoolsConfig : ScriptableObject
    {
        [SerializeField] private ObjectPoolData[] _poolDatas;
    }
}