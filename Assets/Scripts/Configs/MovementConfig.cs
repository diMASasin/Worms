using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "MovementConfig", menuName = "MovementConfig", order = 0)]
    public class MovementConfig : ScriptableObject
    {
        [field: SerializeField] public float Speed { get; private set; }
        [field: SerializeField] public Vector2 LongJumpForce { get; private set; } = new(2, 2);
        [field: SerializeField] public Vector2 HighJumpForce { get; private set; } = new(2, 2);
        [field: SerializeField] public float JumpCooldown { get; private set; } = 0.5f;
        [field: SerializeField] public float MinGroundNormalY { get; private set; } = .65f;
        [field: SerializeField] public float GravityModifier { get; private set; } = 1f;
        [field: SerializeField] public LayerMask LayerMask { get; private set; }
        [field: SerializeField] public GroundCheckerConfig GroundCheckerConfig { get; private set; }
    }
}