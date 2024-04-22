using System;
using DefaultNamespace;
using ScriptBoy.Digable2DTerrain;
using UnityEngine;

namespace Pools
{
    public class ProjectileData
    {
        public Wind Wind { get; set; }

        public ProjectileData(Wind wind)
        {
            Wind = wind;
        }
    }
}