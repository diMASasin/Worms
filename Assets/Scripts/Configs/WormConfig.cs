using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "WormConfig", menuName = "Config/Worm", order = 0)]
    public class WormConfig : ScriptableObject
    {
        [field: SerializeField] public string Name { get; private set; } = "Worm";
        [field: SerializeField] public int MaxHealth { get; private set; } = 100;
        [field: SerializeField] public float RemoveWeaponDelay { get; private set; } = 0.5f;
        [field: SerializeField] public bool ShowCanSpawnCheckerBox { get; private set; } = false;
        [field: SerializeField] public MovementConfig MovementConfig { get; private set; }
        [field: SerializeField] public LayerMask WormLayerMask { get; private set; }
        [field: SerializeField] public LayerMask CurrentWormLayerMask { get; private set; }
    }
}