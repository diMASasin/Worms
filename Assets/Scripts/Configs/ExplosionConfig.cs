using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "ExplosionConfig", menuName = "Config/Explosion", order = 0)]
    public class ExplosionConfig : ScriptableObject
    {
        [field: SerializeField] public float ExplosionForce { get; private set; } = 10;
        [field: SerializeField] public float ExplosionUpwardsModifier { get; private set; } = 4;
        [field: SerializeField] public float ExplosionRadius { get; private set; } = 1;
        [field: SerializeField] public int Damage { get; private set; } = 30;
        [field: SerializeField] public Explosion Prefab { get; private set; }
    }
}