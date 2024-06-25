using Configs;
using Projectiles;

namespace Pools
{
    public interface IProjectilePool
    {
        ProjectileConfig Config { get; }
        void Dispose();
        Projectile Get();
    }
}