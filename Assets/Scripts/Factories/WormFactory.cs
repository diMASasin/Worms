using System;
using System.Collections.Generic;
using Configs;
using DestructibleLand;
using EventProviders;
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

        public event Action<Worm, Color, string> WormCreated;
        public event Action<Worm> DamageTook;
        public event Action<Worm> WormDied;
        public event Action<Worm> InputDelegated;
        public event Action<Worm> InputRemoved;

        public WormFactory(Worm wormPrefab, TerrainWrapper terrain)
        {
            _wormPrefab = wormPrefab;
            _terrain = terrain;
        }

        public Worm Create(Transform parent, Color teamColor, WormConfig config)
        {
            Vector2 position = _terrain.GetRandomSpawnPoint(_wormPrefab.Collider2D.size);
            Worm newWorm = Instantiate(_wormPrefab, position, Quaternion.identity, parent);
            
            newWorm.Init(config);
            
            _worms.Add(newWorm);
            
            newWorm.Died += OnDied;
            newWorm.DamageTook += OnDamageTook;
            newWorm.InputDelegated += OnInputDelegated;
            newWorm.InputRemoved += OnInputRemoved;

            WormCreated?.Invoke(newWorm, teamColor, config.Name + " " + Worm.WormsNumber);
            return newWorm;
        }

        public void Dispose()
        {
            foreach (var worm in _worms)
            {
                worm.DamageTook -= OnDamageTook;
                worm.InputDelegated -= OnInputDelegated;
                worm.InputRemoved -= OnInputRemoved;
            }
        }

        private void OnDamageTook(Worm worm) => DamageTook?.Invoke(worm);

        private void OnInputDelegated(Worm worm) => InputDelegated?.Invoke(worm);

        private void OnInputRemoved(Worm worm) => InputRemoved?.Invoke(worm);

        private void OnDied(Worm worm)
        {
            worm.Died -= OnDied;
            WormDied?.Invoke(worm);
        }
    }
}