using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class Team
{
    [field: SerializeField] public Color Color { get; private set; }
    [field: SerializeField] public string Name { get; private set; }

    private readonly List<Worm> _worms;
    private int _currentWormIndex = -1;

    public int MaxHealth { get; private set; }

    public IReadOnlyList<Worm> Worms => _worms;

    public event UnityAction<Team> Died;
    public event UnityAction<Worm, Team> TurnStarted;
    public event UnityAction<int> HealthChanged;

    public Team(List<Worm> worms, Color color, string teamName)
    {
        _worms = worms;
        Color = color;
        Name = teamName;
        MaxHealth = 0;

        for (int i = 0; i < _worms.Count; i++)
        {
            MaxHealth += _worms[i].MaxHealth;
            _worms[i].Init(color, $"Worm {i + 1}");
            _worms[i].Died += OnWormDied;
            _worms[i].DamageTook += OnDamageTook;
        }
    }

    public void StartTurn()
    {
        _currentWormIndex++;
        if(_currentWormIndex >= _worms.Count)
            _currentWormIndex = 0;

        Worm currentWorm = TryGetCurrentWorm();
        currentWorm.OnTurnStarted();
        TurnStarted?.Invoke(currentWorm, this);
    }

    public Worm TryGetCurrentWorm()
    {
        if (_currentWormIndex >= _worms.Count)
            _currentWormIndex = 0;

        return _worms.Count == 0 ? null : _worms[_currentWormIndex];
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
