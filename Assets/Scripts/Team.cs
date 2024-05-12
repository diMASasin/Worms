using System;
using System.Collections.Generic;
using System.Linq;
using Configs;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class Team
{
    [field: SerializeField] public string Name { get; private set; }

    private readonly List<Worm> _worms;
    private int _currentWormIndex = -1;

    public int MaxHealth { get; private set; }
    public Color Color { get; private set; }

    public List<Worm> Worms => _worms;

    public event UnityAction<Team> Died;
    public event UnityAction<int> HealthChanged;

    public Team(List<Worm> worms, Color color, TeamConfig config)
    {
        _worms = worms;
        Color = color;
        Name = config.Name;
        MaxHealth = 0;

        for (int i = 0; i < _worms.Count; i++)
        {
            MaxHealth += _worms[i].MaxHealth;
            _worms[i].Died += OnWormDied;
            _worms[i].DamageTook += OnDamageTook;
        }
    }

    public bool TryGetNextWorm(out Worm worm)
    {
        _currentWormIndex++;

        if(_currentWormIndex >= _worms.Count)
            _currentWormIndex = 0;

        TryGetCurrentWorm(out worm);

        return worm != null;
    }

    public bool TryGetCurrentWorm(out Worm worm)
    {
        if (_currentWormIndex >= _worms.Count)
            _currentWormIndex = 0;

        worm = _worms.Count == 0 ? null : _worms[_currentWormIndex];

        return worm != null;
    }

    private void OnWormDied(Worm worm)
    {
        worm.DamageTook -= OnDamageTook;
        worm.Died -= OnWormDied;

        if (_currentWormIndex > _worms.IndexOf(worm))
            _currentWormIndex--;

        _worms.Remove(worm);

        OnDamageTook(worm);

        if(_worms.Count <= 0)
            Died?.Invoke(this);
    }

    private void OnDamageTook(Worm damagedWorm)
    {
        var sum = _worms.Sum(worm => worm.Health);
        HealthChanged?.Invoke(sum);
    }
}
