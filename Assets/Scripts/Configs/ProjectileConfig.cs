using Projectiles;
using UnityEditor.Animations;
using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "ProjectileConfig", menuName = "Config/Projectile", order = 0)]
    public class ProjectileConfig : ScriptableObject
    {
        [field: SerializeField] public Projectile ProjectilePrefab { get; private set; }
        [field: SerializeField] public Sprite Sprite{ get; private set; }
        [field: SerializeField] public AnimatorController AnimatorController { get; private set; }
        [field: SerializeField] public ContactFilter2D ContactFilter { get; private set; }
        [field: SerializeField, Header("Behaviour")] public bool WindInfluence { get; private set; } = true;
        [field: SerializeField] public bool LookInVelocityDirection { get; private set; }
        [field: SerializeField, Header("Explosion")] public ExplosionConfig ExplosionConfig { get; private set; }
        [field: SerializeField] public bool ExplodeOnCollision { get; private set; }
        [field: Header("Fragments")]
        [field: SerializeField] public int FragmentsAmount { get; private set; } = 0;
        [field: SerializeField] public int FragmentsDamage { get; private set; } = 10;
        [field: SerializeField] public ProjectileConfig FragmentsConfig { get; private set; }
    }
}