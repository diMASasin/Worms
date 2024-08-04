using ScriptBoy.Digable2DTerrain.Scripts;
using UnityEngine;

namespace DestructibleLand
{
    public class ShovelWrapper : IShovel
    {
        private Shovel _shovel;

        public ShovelWrapper(Shovel shovel)
        {
            _shovel = shovel;
        }

        public void Dig(Vector3 position, float radius)
        {
            _shovel.radius = radius;
            _shovel.transform.position = position;
            _shovel.Dig();
        }
    }
}