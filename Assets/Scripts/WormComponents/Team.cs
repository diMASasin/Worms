using System;
using System.Linq;
using Configs;
using R3;
using UnityEngine;
using UnityEngine.Events;

namespace WormComponents
{
    [Serializable]
    public class Team : IDisposable
    {
        [field: SerializeField] public string Name { get; private set; }

        private readonly CycledList<Worm> _worms;
        private ReactiveProperty<int> _teamHealth = new();

        public int MaxHealth { get; private set; }
        public Color Color { get; private set; }

        public CycledList<Worm> Worms => _worms;
        public ReadOnlyReactiveProperty<int> TeamHealth => _teamHealth;

        public event UnityAction<Team> Died;

        public Team(CycledList<Worm> worms, Color color, TeamConfig config)
        {
            _worms = worms;
            Color = color;
            Name = config.Name;
            MaxHealth = 0;

            foreach (var worm in _worms)
            {
                worm.Health.Subscribe(health => _teamHealth.Value = _worms.Sum(w => w.Health.CurrentValue));
                
                MaxHealth += worm.MaxHealth;
                worm.Died += OnWormDied;
            }
        }

        public void Dispose() => _teamHealth.Dispose();

        private void OnWormDied(Worm worm)
        {
            worm.Died -= OnWormDied;

            _worms.Remove(worm);

            if(_worms.IsEmpty)
                Died?.Invoke(this);
        }
    }
}
