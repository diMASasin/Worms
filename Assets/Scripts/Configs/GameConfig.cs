using Factories;
using ScriptBoy.Digable2DTerrain.Scripts;
using _UI;
using _UI.Message;
using UnityEngine;
using Weapons;
using Wind_;
using WormComponents;

namespace Configs
{
    [CreateAssetMenu(fileName = "BattleConfig", menuName = "Config/Battle", order = 0)]
    public class BattleConfig : ScriptableObject
    {
        [field: Header("Prefabs")]
        [field: SerializeField] public Worm WormPrefab { get; private set; }
        [field: SerializeField] public WormInformationView WormInfoViewPrefab { get; private set; }
        [field: SerializeField] public Arrow ArrowPrefab { get; private set; }
        [field: SerializeField] public Shovel ShovelPrefab { get; private set; }
        [field: SerializeField] public WeaponSelectorItem ItemPrefab { get; private set; }
        [field: SerializeField] public WeaponView WeaponViewPrefab { get; private set; }
        [field: SerializeField] public TeamHealth TeamHealthPrefab { get; private set; }
        [field: SerializeField] public TeamHealthFactory TeamHealthFactoryPrefab { get; private set; }
        [field: SerializeField] public FollowingTimerView FollowingTimerViewPrefab { get; private set; }
        [field: SerializeField] public EndScreen EndScreen { get; private set; }
        [field: SerializeField] public MessageShower MessageShower { get; private set; }

        [field: Header("Configs")]
        [field: SerializeField] public WeaponConfig[] WeaponConfigs { get; private set; }
        [field: SerializeField] public ExplosionConfig ExplosionConfig { get; private set; }
        [field: SerializeField] public WormsSpawnerConfig WormsSpawnerConfig { get; private set; }

        [field: Header("Timers")]
        [field: SerializeField] public TimersConfig TimersConfig { get; private set; } = new();
        
        [field: Header("Wind")]
        [field: SerializeField] public WindData WindData { get; private set; }
        [field: Header("Water")]
        [field: SerializeField] public float WaterStep { get; private set; } = 1;

    }
}