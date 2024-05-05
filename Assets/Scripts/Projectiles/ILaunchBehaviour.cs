using UnityEngine;

namespace Projectiles
{
    public interface ILaunchBehaviour
    {
        void OnLaunch(Vector2 velocity);
    }
}