using Configs;
using ScriptBoy.Digable2DTerrain;
using UnityEngine;
using UnityEngine.UIElements;

namespace DefaultNamespace
{
    public class ShovelWrapper
    {
        private readonly Shovel _shovel;

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