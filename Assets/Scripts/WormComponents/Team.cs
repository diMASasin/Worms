using System;
using System.Linq;
using Configs;
using UnityEngine;
using UnityEngine.Events;

namespace WormComponents
{
    [Serializable]
    public class Team
    {
        [field: SerializeField] public string Name { get; private set; }

        private readonly CycledList<IWorm> _worms;

        public int MaxHealth { get; private set; }
        public Color Color { get; private set; }

        public CycledList<IWorm> Worms => _worms;

        public event UnityAction<Team> Died;
        public event UnityAction<int> HealthChanged;

        public Team(CycledList<IWorm> worms, Color color, TeamConfig config)
        {
            _worms = worms;
            Color = color;
            Name = config.Name;
            MaxHealth = 0;

            foreach (var worm in _worms)
            {
                MaxHealth += worm.MaxHealth;
                worm.Died += OnWormDied;
                worm.DamageTook += OnDamageTook;
            }
        }

        private void OnWormDied(IWorm worm)
        {
            worm.DamageTook -= OnDamageTook;
            worm.Died -= OnWormDied;

            _worms.Remove(worm);

            OnDamageTook(worm);

            if(_worms.IsEmpty)
                Died?.Invoke(this);
        }

        private void OnDamageTook(IWorm arg0)
        {
            var sum = _worms.Sum(worm => worm.Health);
            HealthChanged?.Invoke(sum);
        }
    }
}
