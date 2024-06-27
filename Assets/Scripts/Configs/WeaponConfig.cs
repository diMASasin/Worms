using UnityEngine;
using Weapons;

namespace Configs
{
    [CreateAssetMenu(fileName = "WeaponConfig", menuName = "Config/Weapon", order = 0)]
    public class WeaponConfig : ScriptableObject
    {
        [field: SerializeField] public Weapon WeaponPrefab { get; private set; }
        [field: SerializeField] public int Damage { get; private set; } = 30;
        [field: SerializeField] public float Sencetivity { get; private set; } = 0.01f;
        [field: SerializeField] public float ScopeSensetivity { get; private set; } = 0.7f;
        [field: SerializeField] public float SpeedMultiplier { get; private set; } = 0.03f;
        [field: SerializeField] public float ShotPower { get; private set; } = 5;
        [field: SerializeField] public float MaxShotPower { get; private set; } = 5;
        [field: SerializeField] public Sprite Sprite{ get; private set; }
        [field: SerializeField] public float SpriteUIScale { get; private set; } = 1;
        [field: SerializeField] public ProjectileConfig ProjectileConfig { get; private set; }
    }
}