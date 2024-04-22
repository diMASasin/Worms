using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "GroundCheckerConfig", menuName = "GroundCheckerConfig", order = 0)]
    public class GroundCheckerConfig : ScriptableObject
    {
        [field: SerializeField] public Vector2 Size { get; private set; } = new (0.3f, 0.01f);
        [field: SerializeField] public Vector2 Offset { get; private set; } = new (0, -0.05f);
        [field: SerializeField] public bool ShowGroundCheckerBox { get; private set; } = true;
        [field: SerializeField] public ContactFilter2D ContactFilter2D { get; private set; }
    }
}