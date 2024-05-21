using UnityEngine;

namespace Battle_
{
    public class MapBounds : MonoBehaviour
    {
        [field: SerializeField] public Transform Left { get; private set; }
        [field: SerializeField] public Transform Right { get; private set; }
        [field: SerializeField] public Transform Top { get; private set; }
        [field: SerializeField] public Transform Bottom { get; private set; }
    }
}
