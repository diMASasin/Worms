using UnityEngine;

namespace FollowingObject_
{
    [CreateAssetMenu(fileName = "FollowingObjectConfig", menuName = "Config/FollowingObject")]
    public class FollowingObjectConfig : ScriptableObject
    {
        [field: SerializeField] public bool MoveSmoothly { get; private set; }
        [field: SerializeField] public float Speed { get; private set; } = 10;
        [field: SerializeField] public Vector3 Offset { get; private set; }
    }
}