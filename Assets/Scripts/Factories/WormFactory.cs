using System;
using System.Collections.Generic;
using Configs;
using EventProviders;
using Unity.Mathematics;
using UnityEngine;
using WormComponents;
using Object = UnityEngine.Object;

namespace Factories
{
    public class WormFactory : IWormEventsProvider, IDisposable
    {
        private readonly Worm _wormPrefab;
        private readonly List<Worm> _worms = new();

        public event Action<Worm, Color, string> WormCreated;
        public event Action<Worm> WormDied;
        public event Action<Worm> WormDamageTook;

        public WormFactory(Worm wormPrefab)
        {
            _wormPrefab = wormPrefab;
        }

        public Worm Create(Transform parent, Color teamColor, WormConfig config, Func<Vector2> getSpawnPoint)
        {
            var newWorm = Object.Instantiate(_wormPrefab, getSpawnPoint(), quaternion.identity, parent);

            newWorm.Init(config);
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