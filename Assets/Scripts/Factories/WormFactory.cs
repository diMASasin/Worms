using System;
using System.Collections.Generic;
using Configs;
using DestructibleLand;
using EventProviders;
using UnityEngine;
using WormComponents;
using Object = UnityEngine.Object;

namespace Factories
{
    public class WormFactory : IWormEvents, IDisposable
    {
        private readonly Worm _wormPrefab;
        private readonly TerrainWrapper _terrain;
        private readonly List<Worm> _worms = new();

        public event Action<IWorm, Color, string> WormCreated;
        public event Action<IWorm> WormDied;
        public event Action<IWorm> WormDamageTook;

        public WormFactory(Worm wormPrefab, TerrainWrapper terrain)
        {
            _wormPrefab = wormPrefab;
            _terrain = terrain;
        }

        public Worm Create(Transform parent, Color teamColor, WormConfig config)
        {
            Vector2 position = _terrain.GetRandomSpawnPoint(_wormPrefab.Collider2D.size);
            var newWorm = Object.Instantiate(_wormPrefab, position, Quaternion.identity, parent);
            newWorm.Init(config);
            newWorm.FreezePosition();
            
            _worms.Add(newWorm);
            
            newWorm.Died += OnDied;
            newWorm.DamageTook += OnDamageTook;

            WormCreated?.Invoke(newWorm, teamColor, config.Name + " " + Worm.WormsNumber);
            return newWorm;
        }

        public void Dispose()
        {
            foreach (var worm in _worms) 
                worm.DamageTook -= OnDamageTook;
        }

        private void OnDamageTook(IWorm worm)
        {
            WormDamageTook?.Invoke(worm);
        }

        private void OnDied(IWorm worm)
        {
            worm.Died -= OnDied;
            WormDied?.Invoke(worm);
        }
    }
}