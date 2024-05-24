using System;
using System.Collections.Generic;
using Configs;
using EventProviders;
using MovementComponents;
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

        public event Action<IWorm, Color, string> WormCreated;
        public event Action<IWorm> WormDied;
        public event Action<IWorm> WormDamageTook;

        public WormFactory(Worm wormPrefab)
        {
            _wormPrefab = wormPrefab;
        }

        public Worm Create(Transform parent, Color teamColor, WormConfig config, Func<Vector2> getSpawnPoint)
        {
            var newWorm = Object.Instantiate(_wormPrefab, getSpawnPoint(), Quaternion.identity, parent);
            
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