using System;
using System.Collections.Generic;
using System.Linq;
using Configs;
using UnityEngine;
using UnityEngine.Events;
using WormComponents;

[Serializable]
public class Team
{
    [field: SerializeField] public string Name { get; private set; }

    private readonly CycledList<Worm> _worms;
    private int _currentWormIndex = -1;

    public int MaxHealth { get; private set; }
    public Color Color { get; private set; }

    public CycledList<Worm> Worms => _worms;

    public event UnityAction<Team> Died;
    public event UnityAction<int> HealthChanged;

    public Team(CycledList<Worm> worms, Color color, TeamConfig config)
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

    public bool TryGetNextWorm(out Worm worm)
    {
        worm = _worms.Next();
        
        return worm != null;
    }

    private void OnWormDied(Worm worm)
    {
        worm.DamageTook -= OnDamageTook;
        worm.Died -= OnWormDied;

        _worms.Remove(worm);

        OnDamageTook(worm);

        if(_worms.IsEmpty)
            Died?.Invoke(this);
    }

    private void OnDamageTook(Worm damagedWorm)
    {
        var sum = _worms.Sum(worm => worm.Health);
        HealthChanged?.Invoke(sum);
    }
}
