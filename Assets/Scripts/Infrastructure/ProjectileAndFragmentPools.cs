using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pools;
using Projectiles;

namespace Infrastructure
{
    public class ProjectileAndFragmentPools : IProjectilesCount
    {
        private readonly List<ProjectilePool> _pools = new();

        public int Count => _pools.Sum(pool => pool.Count);

        public ProjectileAndFragmentPools(IEnumerable<ProjectilePool> projectilePools, IEnumerable<ProjectilePool> fragmentPools)
        {
            _pools.AddRange(projectilePools.Concat(fragmentPools));
        }
    }
}