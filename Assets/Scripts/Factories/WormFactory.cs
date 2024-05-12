using System;
using System.Collections.Generic;
using Configs;
using EventProviders;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Factories
{
    public class WormFactory : IWormEventsProvider, IDisposable
    {
        private Worm _wormPrefab;
        private List<Worm> _worms = new();

        public event Action<Worm> WormDied;
        public event Action<Worm> WormDamageTook;

        public WormFactory(Worm wormPrefab)
        {
            _wormPrefab = wormPrefab;
        }

        public Worm Create(Transform parent, Color teamColor, WormConfig config)
        {
            var newWorm = Object.Instantiate(_wormPrefab, parent);

            newWorm.Init(teamColor, config.Name, config);
            _worms.Add(newWorm);
            
            newWorm.Died += OnDied;
            newWorm.DamageTook += OnDamageTook;

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