using UnityEngine;

namespace Projectiles.Behaviours.LaunchBehaviour
{
    public interface ILaunchBehaviour
    {
        void OnLaunch(Vector2 velocity);
    }
}