using UnityEngine;

namespace DestructibleLand
{
    public interface IShovel
    {
        void Dig(Vector3 position, float radius);
    }
}