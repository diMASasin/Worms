using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "ProjectileConfig", menuName = "ProjectileConfig", order = 0)]
    public class ProjectileConfig : ScriptableObject
    {
        [field: SerializeField] public int Damage { get; private set; } = 30;
        [field: SerializeField] public float ExplosionForce { get; private set; } = 10;
        [field: SerializeField] public float ExplosionUpwardsModifier { get; private set; } = 4;
        [field: SerializeField] public float ExplosionRadius { get; private set; } = 1;
        [field: SerializeField] public float ColliderRadius { get; private set; } = 1;
        [field: SerializeField] public bool WindInfluence { get; private set; } = true;
        [field: SerializeField] public float ExplodeDelay { get; private set; } = 0;
        [field: SerializeField] public int FragmentsAmount { get; private set; } = 0;
        [field: SerializeField] public Sprite Sprite{ get; private set; }
    }
}