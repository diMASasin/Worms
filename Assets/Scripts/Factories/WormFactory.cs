using System;
using System.Collections.Generic;
using Configs;
using DestructibleLand;
using EventProviders;
using InputService;
using Services;
using UltimateCC;
using UnityEngine;
using WormComponents;
using static UnityEngine.Object;

namespace Factories
{
    public class WormFactory : IWormEvents, IDisposable
    {
        private readonly Worm _wormPrefab;
        private readonly TerrainWrapper _terrain;
        private readonly List<Worm> _worms = new();
        private readonly IInput _input;

        public event Action<Worm, Color, string> WormCreated;
        public event Action<Worm> WormDied;
        public event Action<Worm> WormDamageTook;

        public WormFactory(Worm wormPrefab, TerrainWrapper terrain, AllServices services)
        {
            _wormPrefab = wormPrefab;
            _terrain = terrain;
            _input = services.Single<IInput>();
        }

        public Worm Create(Transform parent, Color teamColor, WormConfig config)
        {
            Vector2 position = _terrain.GetRandomSpawnPoint(_wormPrefab.Collider2D.size);
            Worm newWorm = Instantiate(_wormPrefab, position, Quaternion.identity, parent);
            
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

        private void OnDamageTook(Worm worm)
        {
            WormDamageTook?.Invoke(worm);
        }

        private void OnDied(Worm worm)
        {
            worm.Died -= OnDied;
            WormDied?.Invoke(worm);
        }
    }
}