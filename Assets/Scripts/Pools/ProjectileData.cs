using System;
using ScriptBoy.Digable2DTerrain;
using UnityEngine;

namespace Pools
{
    public class ProjectileData
    {
        public ObjectPool<Explosion> ExplosionsPool { get; set; }
        public Shovel Shovel { get; set; }
        public Wind Wind { get; set; }
        public ProjectilePool FragmentsPool { get; set; }

        public ProjectileData(ObjectPool<Explosion> explosionsPool, Shovel shovel, Wind wind, ProjectilePool fragmentsPool)
        {
            ExplosionsPool = explosionsPool;
            Shovel = shovel;
            Wind = wind;
            FragmentsPool = fragmentsPool;
        }
    }
}