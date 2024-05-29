using Factories;
using ScriptBoy.Digable2DTerrain.Scripts;
using UI;
using UnityEngine;
using Weapons;
using Wind_;
using WormComponents;

namespace Configs
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Config/Game", order = 0)]
    public class GameConfig : ScriptableObject
    {
        [field: Header("Prefabs")]
        [field: SerializeField] public FollowingTimerView FollowingTimerViewPrefab { get; private set; }
        [field: SerializeField] public Worm WormPrefab { get; private set; }
        [field: SerializeField] public WormInformationView WormInfoViewPrefab { get; private set; }
        [field: SerializeField] public Arrow ArrowPrefab { get; private set; }
        [field: SerializeField] public Shovel ShovelPrefab { get; private set; }
        [field: SerializeField] public WeaponSelectorItem ItemPrefab { get; private set; }
        [field: SerializeField] public WeaponView WeaponViewPrefab { get; private set; }
        [field: SerializeField] public TeamHealth TeamHealthPrefab { get; private set; }
        [field: SerializeField] public TeamHealthFactory TeamHealthFactoryPrefab { get; private set; }

        [field: Header("Configs")]
        [field: SerializeField] public WeaponConfig[] WeaponConfigs { get; private set; }
        [field: SerializeField] public ExplosionConfig ExplosionConfig { get; private set; }
        [field: Header("Timers")]
        [field: SerializeField] public TimersConfig TimersConfig { get; private set; }
        
        [field: Header("Wind")]
        [field: SerializeField] public WindData WindData { get; private set; }
        [field: Header("Water")]
        [field: SerializeField] public float WaterStep { get; private set; } = 1;

    }
}