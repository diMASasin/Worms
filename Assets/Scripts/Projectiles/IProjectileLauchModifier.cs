using UnityEngine;

namespace Projectiles
{
    public interface IProjectileLauchModifier
    {
        void OnLaunch(Vector2 velocity);
    }
}