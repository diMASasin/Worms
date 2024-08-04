using Factories;
using ScriptBoy.Digable2DTerrain.Scripts;
using UI_;
using UI_.Message;
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
        [field: SerializeField] public WindSettings WindSettings { get; private set; }
        [field: Header("Water")]
        [field: SerializeField] public Material WaterMaterial { get; private set; }
        [field: SerializeField] public float WaterStep { get; private set; } = 1;

        [field: SerializeField, Range(0, 1)] public float MaxWaveHeight { get; private set; } = 0.5f;

    }
}